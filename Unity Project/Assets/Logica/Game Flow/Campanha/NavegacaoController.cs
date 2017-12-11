using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class NavegacaoController : MonoBehaviour {
    public static NavegacaoController instancia;
    public GameObject player;
    public int velocidadeDeRotacao;
    public int velocidadeDeMovimentacao = 3;
    public List<Node> nodes;//Node 0 e sempre o Start Point, o ultimo node e sempre o do Boss
    //Feedback na interface
    public WorldMapNodeInfo holderDeInformacoes;
    public GameObject holderDeInformacoesGameObject;
    //Para animação
    public RuntimeAnimatorController animatorNodeInimigo;
    public RuntimeAnimatorController animatorNodeAliado;
    public bool girando;
    public bool orientacao;//True para direita, false para esquerda
    public bool emMovimento;

    public Text feedbackCombustivel;
    public Text feedbackReservas;
    public Text feedbackMunicao;
    public Text feedbackDias;
    public GameObject mascara;//Mascara para poder sair da informações do Node se Precisar
    public GameObject protetorDeTela;//Como se carrega a batalha em modo aditivo, os botões da interface ainda estão lá
                                     //O protetor de tela ta lá pra não deixar os OnClickListener ouvirem nada

    public AudioSource BGM;
    //Para controlar de termino da campanha
    public GameObject telaDeResultado;
    public GameObject mascaraDeResultado;
    public GameObject terminarCampanhaButton;
    private bool vitoriaAdiquirida = false;
    public Sprite[] vitoriaOuDerrota = new Sprite[2];
    public Sprite[] imagemsDemonstrativas = new Sprite[2];
    public Image imagemDemonstrativa;
    public Image imagemDeResultado;
    public Button continuarCampanhaButton;
    //Para navegação
    private Node noAtual;
    private Node noPrevio;
    //Para controle de dias
    private int diaAtual;
    private int diasMaximosDaCampanhas;

    void Awake(){
        instancia = this;
    }

    void Start(){
        diasMaximosDaCampanhas = BancoDeDados.instancia.campanhasDoJogo[GameController.instancia.campanhaSelecionada].diasMaximo;
        diaAtual=0;
        noPrevio = nodes[0];
        noAtual = nodes[0];
        holderDeInformacoes.popularInfoBasica(noAtual);
        holderDeInformacoes.popularSuprimentos();
        player.transform.position = nodes[0].transform.position;
        atualiazarHeader();
        habilitarMovimentos();
    }

    void Update(){
        if (girando){
            if (orientacao && player.transform.eulerAngles.y != 180) {
                player.transform.Rotate(Vector3.up * velocidadeDeRotacao * Time.deltaTime);
                if(player.transform.eulerAngles.y <= 190 && player.transform.eulerAngles.y >=170) {
                    player.transform.rotation = Quaternion.Euler(0, 180, 0);
                    girando = false;
                    emMovimento = true;
                }
            }
            else if (!orientacao && player.transform.eulerAngles.y != 0){
                player.transform.Rotate(Vector3.up * velocidadeDeRotacao * Time.deltaTime);
                if (player.transform.eulerAngles.y <= 10 && player.transform.eulerAngles.y >= 0){
                    player.transform.rotation = Quaternion.Euler(0, 0, 0);
                    girando = false;
                    emMovimento = true;
                }
            }
            else{ //Ele já está no angulo certinho
                girando = false;
                emMovimento = true;
            }        
        }
        else if (emMovimento){
            if (!SoundPlayer.instancia.fonteDeSom.isPlaying) { SoundPlayer.instancia.TanqueAvanco(); }
            player.transform.position += (noAtual.transform.position - player.transform.position).normalized * velocidadeDeMovimentacao * Time.deltaTime;
            if (Vector3.Distance(noAtual.transform.position, player.transform.position) <= 0.1f){
                player.transform.position = noAtual.transform.position;
                emMovimento = false;
                //Chegou no node de destino, mostrando informacoes do node
                mascara.SetActive(true);
                holderDeInformacoesGameObject.SetActive(true);
                holderDeInformacoes.popularInfoBasica(noAtual);
                //Definindo a informação a ser mostrada
                if (!noAtual.conquistado){//Moveu pra um node agressivo! preparar para Batalha
                    holderDeInformacoes.popularInteligencia();
                    SoundHolder.instancia.AlarmeMilitar();
                }
                else{//Um node sem influencia inimiga, mostrar opções de suprimento
                    holderDeInformacoes.popularSuprimentos();
                    if (noAtual.recentementeConquistada) SoundHolder.instancia.sussuros();
                    else SoundHolder.instancia.cidadeNormal();
                }
                habilitarMovimentos();
            }
        }
    }

    public void habilitarMovimentos(){
        foreach(Node no in nodes){
            if (noAtual.conquistado == true && noAtual.vizinhos.Contains(no)){
                no.gameObject.GetComponent<Button>().interactable = true;
            }
            else {
                no.gameObject.GetComponent<Button>().interactable = false;
            }            
        }
    }
    
    bool checarVitoria(){
        List<int[]> victoryConditions = BancoDeDados.instancia.campanhasDoJogo[GameController.instancia.campanhaSelecionada].victoryConditions ;
        vitoriaAdiquirida = false;
        switch (GameController.instancia.campanhaSelecionada){
            case 0:
                for(int i = 0; i < victoryConditions.Count;i++){
                    vitoriaAdiquirida = false;
                    bool condicaoCompletada = true;
                    for (int j = 0; j < victoryConditions[i].Length; j++){
                        if (nodes.Where(x => x.nodeNoMundo0 == (Mundo0)victoryConditions[i][j] && x.conquistado).Count() == 0) {
                            condicaoCompletada = false;
                        }
                        if (!condicaoCompletada) { break; }
                    }
                    if (condicaoCompletada) {vitoriaAdiquirida = true; break; }
                }
                break;
            case 1:
                for (int i = 0; i < victoryConditions.Count;i++){
                    vitoriaAdiquirida = false;
                    bool condicaoCompletada = true;
                    for (int j = 0; j < victoryConditions[i].Length; j++){
                        if (nodes.Where(x => x.nodeNoMundo1 == (Mundo1)victoryConditions[i][j] && x.conquistado).Count() == 0){
                            condicaoCompletada = false;
                        }
                        if (!condicaoCompletada) { break; }
                    }
                    if (condicaoCompletada) { vitoriaAdiquirida = true; break; }
                }
                break;
        }
        return vitoriaAdiquirida;
    }
    
    public void fecharInformacoesExtras(){
        holderDeInformacoes.gameObject.SetActive(false);
        mascara.SetActive(false);
    }

    public void moverPlayerParaNode(int qual){
        diaAtual++;
        Node destino = nodes[qual];

        girando = true;
        if (noAtual.transform.position.x >= destino.transform.position.x)
            orientacao = false;
        else
            orientacao = true;

        noPrevio = noAtual;
        noAtual = destino;
        
        GameController.instancia.nodeAtual = noAtual;
        atualiazarHeader();
    }

    public void toBattle(bool rush){
        holderDeInformacoesGameObject.SetActive(false);
        protetorDeTela.SetActive(true);
        BGM.Pause();
        GameController.instancia.onBattle = true;
        GameController.instancia.rushFront = rush;
        SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
        StartCoroutine(esperandoBatalhaTerminar());        
    }

    IEnumerator esperandoBatalhaTerminar(){
        while (GameController.instancia.onBattle){
            yield return new WaitForSeconds(0.3f);
        }
        batalhaTerminada();
        yield break;
    }

    public void batalhaTerminada(){
        holderDeInformacoesGameObject.SetActive(true);
        holderDeInformacoes.popularInfoBasica(noAtual);
        holderDeInformacoes.popularSuprimentos();
        SoundPlayer.instancia.fonteDeSom = this.GetComponent<AudioSource>();
        BGM.UnPause();

        protetorDeTela.SetActive(false);     
        if (GameController.instancia.victory){
            noAtual.gameObject.GetComponent<Animator>().runtimeAnimatorController = animatorNodeAliado;
            noAtual.nodeConquistado();
            //Animação de conquista
            if (checarVitoria()){
                holderDeInformacoesGameObject.SetActive(false);
                PopularFimDaCampanha();
            }
        }
        else {
            retirada();
            //Animação de Retirada
        }
        diaAtual += GameController.instancia.diasEmBatalha - 1;
        GameController.instancia.diasEmBatalha = 0;
        newDay();
        habilitarMovimentos();
    }

    public void retornarParaHQ(){
        SceneManager.LoadScene("HQ");
    }

    public void retirada(){
        fecharInformacoesExtras();
        moverPlayerParaNode(nodes.FindIndex(no => no == noPrevio));
    }

    public void PopularFimDaCampanha(){
        telaDeResultado.SetActive(true);
        mascaraDeResultado.SetActive(true);
        if (vitoriaAdiquirida){
            //Se ele conseguiu vencer a campanha mais recentemente aberta....
            if(GameController.instancia.campanhaAtiva == GameController.instancia.campanhaSelecionada){
                GameController.instancia.campanhaAtiva++;
                GameController.instancia.ativarCampanhas();
            }
            imagemDeResultado.sprite = vitoriaOuDerrota[0];
            imagemDemonstrativa.sprite = imagemsDemonstrativas[0];
            if (diaAtual >= diasMaximosDaCampanhas) { continuarCampanhaButton.interactable = false; }
            else { continuarCampanhaButton.interactable = true; }
        }
        else{
            imagemDeResultado.sprite = vitoriaOuDerrota[1];
            imagemDemonstrativa.sprite = imagemsDemonstrativas[1];
            continuarCampanhaButton.interactable = false;
        }
    }

    public void abrirFimDaCampanhaNovamente(){
        telaDeResultado.SetActive(true);
        mascaraDeResultado.SetActive(true);
    }

    public void continuarCampanha(){
        if (!vitoriaAdiquirida) { return; }
        if (!terminarCampanhaButton.activeSelf) { terminarCampanhaButton.SetActive(true); }
        telaDeResultado.SetActive(false);
        mascaraDeResultado.SetActive(false);
    }
    
    public void entrarNoNode(){
        if (holderDeInformacoesGameObject.activeSelf) { return; }
        else {
            holderDeInformacoesGameObject.SetActive(true);
        }
    }

    public void atualiazarHeader(){
        feedbackCombustivel.text = GameController.instancia.Combustivel.ToString();
        feedbackMunicao.text = GameController.instancia.Municao.ToString();
        feedbackReservas.text = GameController.instancia.MaoDeObra.ToString();
        feedbackDias.text = (diasMaximosDaCampanhas-diaAtual).ToString();
    }  

    public void newDay(){
        diaAtual++;
        if(diaAtual>diasMaximosDaCampanhas) {
            //Falhou a campanha
            //Animação de Derrota?
            PopularFimDaCampanha();
        }
        else{
            atualiazarHeader();
            operacoesDoNovoDia();
            aplicarAtrito();
        }
    }

    public void operacoesDoNovoDia(){//Tudo que os nodes sofrem depois de um dia
        foreach(Node no in nodes){
            no.calcularTemperatura();
            if (no.recentementeConquistada) { no.nodePacificado(); }
        }
    }    
    
    private void aplicarAtrito(){
        if (noAtual.parteDoReich)               { return;    }
        else if (noAtual.temperaturaAtual < 0)  { atrito(5); }
        else if (noAtual.temperaturaAtual < 10) { atrito(4); }
        else if (noAtual.temperaturaAtual < 20) { atrito(3); }
        else if (noAtual.temperaturaAtual < 25) { atrito(2); }
        else if (noAtual.temperaturaAtual < 28) { atrito(1); }
        else if (noAtual.temperaturaAtual < 32) { atrito(2); }
        else if (noAtual.temperaturaAtual < 40) { atrito(3); }
        else if (noAtual.temperaturaAtual < 48) { atrito(4); }
        else                                    { atrito(5); }
    }

    private void atrito(int quanto){
        if (quanto == 0) return;

        foreach(Unidade u in GameController.instancia.primeiroBatalhao){
            u.status[13] -= quanto;
            u.status[15] -= quanto;
            u.status[17] -= quanto;
        }
         foreach(Unidade u in GameController.instancia.segundoBatalhao){
            u.status[13] -= quanto;
            u.status[15] -= quanto;
            u.status[17] -= quanto;
        }
          foreach(Unidade u in GameController.instancia.terceiroBatalhao){
            u.status[13] -= quanto;
            u.status[15] -= quanto;
            u.status[17] -= quanto;
        }
    }
    
}
