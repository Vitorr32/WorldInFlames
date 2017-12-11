using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour {
    public static InterfaceController instancia;
    //Para controlar o que está sendo mostrado ao usuario
    public GameObject acoesDoPlayer;
    public GameObject botoesDeAcao;
    public GameObject promptHolder;
    public GameObject endGameHolder;
    //Para amostragem da unidade atual
    public InformacoesUnidadeBatalha informacoesDaUnidadeAtual;
    //Para tela de resultado, parte de relatorio
    public Text danoInfligido;
    public Text danoRecebido;
    public Text inimigosDestruidos;
    public Text unidadesPerdiadas;
    public Image rankDaBatalha;
    public Sprite[] imagensDeRanking;
	//Para tela de resultado, parte de operação
	public Text experienciaGanha;
	public Text inspiracaoGanha;
	public Image cartaDaUnidadeRecrutada;
	public Sprite cartaDeUnidadeVazia;
	public Text healthPointsNovaUnidade;
	public Text softDamageNovaUnidade;
	public Text hardDamageNovaUnidade;
    //Para o fluxo da batalha
    public AudioSource BGM;
    public Text feedbackTileEspecial;
    public Text horaAtual;
    public Text minutosFaltando;
    public GameObject holderDeChegadaFront;
    public Image noiteOuDiaFeedback;
    public Sprite[] noiteOuDiaImagens = new Sprite[2];
    //Ações que o jogador pode fazer
    public Button atacar;
    public Button mover;
    public Button posicionar;
    public Button allSet;

    void Awake(){
        instancia=this;
    }

    public void tocarBGM(AudioClip musicaATocar){
        BGM.clip = musicaATocar;
        BGM.Play();
    }
    
    public void TerminarBatalha(int battleRating){
        if(!endGameHolder.activeSelf)endGameHolder.SetActive(true);
        float multiplicadorExperiencia=0;
        switch(battleRating) {
            case 5: multiplicadorExperiencia = 2.5f; rankDaBatalha.sprite = imagensDeRanking[4]; GameController.instancia.Inspiracao+=2; inspiracaoGanha.text = "+2"; break;
            case 4: multiplicadorExperiencia = 1.5f; rankDaBatalha.sprite = imagensDeRanking[3]; GameController.instancia.Inspiracao+=1; inspiracaoGanha.text = "+1"; break;
            case 3: multiplicadorExperiencia = 1.0f; rankDaBatalha.sprite = imagensDeRanking[2]; inspiracaoGanha.text = "0"; break;
            case 2: multiplicadorExperiencia = 0.75f; rankDaBatalha.sprite = imagensDeRanking[1]; GameController.instancia.Inspiracao -= 1; inspiracaoGanha.text = "-1"; break;
            case 1: multiplicadorExperiencia = 0.5f; rankDaBatalha.sprite = imagensDeRanking[0]; GameController.instancia.Inspiracao -= 2; inspiracaoGanha.text = "-2"; break;
        }
        int experienciafinal = (int)(GameController.instancia.nodeAtual.expBase * multiplicadorExperiencia);
        experienciaGanha.text = experienciafinal.ToString();
        
        GameController.instancia.darExperienciaUnidades(experienciafinal);
        Unidade unidadeDropada = GameController.instancia.Drop(battleRating);
        if (GameController.instancia.AdicionarUnidadeAoJogador(unidadeDropada) != -1 ){
            PopularNovaUnidadeDropada(unidadeDropada);
        }

        danoInfligido.text = BatalhaController.instancia.danoTotalAliado.ToString();
        danoRecebido.text = BatalhaController.instancia.danoTotalInimigo.ToString();
        inimigosDestruidos.text = BatalhaController.instancia.numeroDeInimigosDestruidos.ToString();
        unidadesPerdiadas.text = BatalhaController.instancia.numeroDeAliadosDestruidos.ToString();

    }

    private void PopularNovaUnidadeDropada(Unidade unidade){
        if(unidade != null){
            cartaDaUnidadeRecrutada.sprite = BancoDeDados.instancia.visualPrincipalUnidadesDoJogo[unidade.idNoBancoDeDados];
            healthPointsNovaUnidade.text = unidade.status[0].ToString();
            softDamageNovaUnidade.text = unidade.status[5].ToString();
            hardDamageNovaUnidade.text = unidade.status[6].ToString();
        }
        else{
            cartaDaUnidadeRecrutada.sprite = cartaDeUnidadeVazia;
            healthPointsNovaUnidade.text = " - ";
            softDamageNovaUnidade.text = " - ";
            hardDamageNovaUnidade.text = " - ";
        }
    }

    public void terminouVez(){
        if (botoesDeAcao.activeSelf) botoesDeAcao.SetActive(false);
        UnidadeNoTabuleiro atual = BatalhaController.instancia.unidadesNoTabuleiro[BatalhaController.instancia.unidadeAtualIndex];
        tooglarAcoesDoPlayer(!atual.inimigo);
        mostrarHoras();
        atualizarCountdown();
    }

    public void mostrarHoras(){
        string horarioString;
        if(BatalhaController.instancia.horarioAtual< 1000) { horarioString = "0" + BatalhaController.instancia.horarioAtual; }
        else { horarioString = BatalhaController.instancia.horarioAtual.ToString(); }
        horarioString.Insert(1, ":");
        horaAtual.text = horarioString;

        if (BatalhaController.instancia.noite) { noiteOuDiaFeedback.sprite = noiteOuDiaImagens[0]; }
        else { noiteOuDiaFeedback.sprite = noiteOuDiaImagens[1]; }
    }

    public void atualizarCountdown(){
        if(BatalhaController.instancia.faseDeBatalha == 3) {
            if (holderDeChegadaFront.gameObject.activeSelf) holderDeChegadaFront.gameObject.SetActive(false);
            return;
        }
        if (!holderDeChegadaFront.gameObject.activeSelf) holderDeChegadaFront.gameObject.SetActive(true);
            minutosFaltando.text = (BatalhaController.instancia.chegadaFront - BatalhaController.instancia.horarioAtual) + "min";
        
    }
    public void mostrarAcoes(){
        UnidadeNoTabuleiro atual = BatalhaController.instancia.unidadesNoTabuleiro[BatalhaController.instancia.unidadeAtualIndex];
        if (atual.inimigo) { desativarAcoesDoPlayer(); }
        else               { ativarAcoesDoPlayer(); }
        atual.gridDeMovimento = false;
        atual.gridDeAtaque = false;
        BatalhaController.instancia.removerTileHighlights();
        tooglarBotoesDeAcoes(); 
    }

    private void tooglarAcoesDoPlayer(bool ativar){
        if (ativar) { ativarAcoesDoPlayer(); }
        else        { desativarAcoesDoPlayer(); }
    }

    private void ativarAcoesDoPlayer(){
        if (!acoesDoPlayer.activeSelf) { acoesDoPlayer.SetActive(true); }
    }

    private void desativarAcoesDoPlayer(){
        if (acoesDoPlayer.activeSelf) { acoesDoPlayer.SetActive(false); }
    }

    private void tooglarBotoesDeAcoes(){
        UnidadeNoTabuleiro atual = BatalhaController.instancia.unidadesNoTabuleiro[BatalhaController.instancia.unidadeAtualIndex];
        if (!botoesDeAcao.activeSelf) {
            botoesDeAcao.SetActive(true);
            if (atual.artilharia){
                posicionar.gameObject.SetActive(true);
                if (atual.preparada) { atacar.interactable = true; posicionar.interactable = false; }
                else { atacar.interactable = false; posicionar.interactable = true; }
            }
            else{
                posicionar.gameObject.SetActive(false);
            }
        }
        else { botoesDeAcao.SetActive(false); }

    }

    public void desativarOpcaoDePosicionamento(){
        posicionar.interactable = false;
    }

    public void mostrarPromptTileEspecial(TileMechanic tm) {
        promptHolder.SetActive(true);
        switch(tm){
            case TileMechanic.PLAYER_RETREAT_LINE:
                feedbackTileEspecial.text="Você quer mandar a unidade atual que sofreu grandes danos retornar ao HQ?";
                break;
            case TileMechanic.PLAYER_ATTACK_SUPPORT:
                feedbackTileEspecial.text="Você quer mandar a unidade atual avançar por entre as linhas inimigas e atacar a sua retaguarda?";
                break;
        }
    }

    public void escoderPromptTileEspecial(){
        promptHolder.SetActive(false);
    }

    public void ModoDeAtaque() {
        UnidadeNoTabuleiro atual = BatalhaController.instancia.unidadesNoTabuleiro[BatalhaController.instancia.unidadeAtualIndex];
        if (!atual.gridDeAtaque){
            BatalhaController.instancia.removerTileHighlights();
            atual.gridDeMovimento = false;
            atual.gridDeAtaque = true;
            //Se a unidade atual tem munição
            if(atual.unidadeAssociada.status[15] > 0)
                BatalhaController.instancia.highlightTilesAt(atual.gridPosition, BatalhaController.instancia.TileGridAtaque, atual.unidadeAssociada.status[4]);
            else
                BatalhaController.instancia.highlightTilesAt(atual.gridPosition, BatalhaController.instancia.TileGridAtaque, 1);
        }
        else{
            atual.gridDeMovimento = false;
            atual.gridDeAtaque = false;
            BatalhaController.instancia.removerTileHighlights();
        }
        if (botoesDeAcao.activeSelf) botoesDeAcao.SetActive(false);
    }

    public void ModoDeMovimento(){
        UnidadeNoTabuleiro atual = BatalhaController.instancia.unidadesNoTabuleiro[BatalhaController.instancia.unidadeAtualIndex];
        if (!atual.gridDeMovimento){
            BatalhaController.instancia.removerTileHighlights();
            atual.gridDeMovimento = true;
            atual.gridDeAtaque = false;
            //Se a unidade atual tem combustivel
            if (atual.unidadeAssociada.status[13] > 0)
                BatalhaController.instancia.highlightTilesAt(atual.gridPosition, BatalhaController.instancia.TileGridMovimento, atual.unidadeAssociada.status[3]);
            else
                BatalhaController.instancia.highlightTilesAt(atual.gridPosition, BatalhaController.instancia.TileGridMovimento, 1);
        }
        else{
            atual.gridDeMovimento = false;
            atual.gridDeAtaque = false;
            BatalhaController.instancia.removerTileHighlights();
        }
        if (botoesDeAcao.activeSelf) botoesDeAcao.SetActive(false);
    }

    public void permitirTerminarPreparacao(){
        allSet.interactable = true;
    }

    public void bloquearAteTerminarPreparacao(){
        allSet.gameObject.SetActive(true);
        allSet.interactable = false;
    }

    public void toogleAllSet(){
        if (allSet.gameObject.activeSelf) { allSet.gameObject.SetActive(false); }
        else { allSet.gameObject.SetActive(true); }
    }

    public void AllSet() {
        BatalhaController.instancia.comecarFase();     
        toogleAllSet();
    }
}
