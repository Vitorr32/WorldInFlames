using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class ArsenalController : MonoBehaviour {
    public GameObject[] nomeSelecionaveis;
    public Image imagenDaUnidade;
    public GameObject[] equipamentos;
    public Image[] equipamentosIcones = new Image[4];
    public Button[] removerEquipamentos = new Button[4];
    public GameObject[] equipamentosIdle;
    public Text paginacaoEquipamentoFeedback;
    public Text paginacaoUnidadeFeedback;
    public Button[] paginacaoEquipamento;
    public Button[] paginacaoUnidade;
    public Button[] batalhoes;
    public GameObject trocaDeEquipamentosHolder;
    public GameObject equipamentoInterface;
    public GameObject paginacaoUnidadeHolder;
    public GameObject paginacaoEquipamentoHolder;

    public GameObject mascaraForaDoHolder;
    public GameObject mascaraDentroDoHolder;
    public CartaEquipamentoComparacao comparadorGameObject;
    public Text[] status;

    public SpriteState naoAtribuidoState;
    public SpriteState atribuidoState;

    public Sprite emptySelection;
    public Image cartaDaUnidade;
    public Sprite CartaVoidUnidade;

    public Sprite nenhumaClasseSprite;
    public Sprite atribuidoSprite;
	public Sprite naoAtribuidoSprite;
	public Sprite desabilitadoSprite;

    bool naComparacao = false;

    public Sprite[] batalhaoSelecionado = new Sprite[4];
    public Image batalhaoVisualFeedback;
    
    private int qualBatalhao;//1 pro primeiro, 2 pro segundo, 3 pro terceiro e 4 para reservas
    private int paginaUni = 0;//Como so poderam ser mostrados 3 unidades de uma vez, e nescessario paginação
    private int paginaEqui = 0;//So poderam ser mostrados 10 equipamentos, portanto 12 paginas

    private int slotClicado;
    private int equipamentoNoArsenalClicado;
    private Unidade ChosenOne;

    public void FirstEnter(){
        TrocarBatalhao(1);
        PopularListaDoPrimeiroBatalhao();
        PopularListaDeEquipamentos();
        if (GameController.instancia.primeiroBatalhao.Count != 0)TrocarChosenOne(0);

        batalhaoVisualFeedback.sprite = batalhaoSelecionado[qualBatalhao - 1];
        paginacaoUnidade[0].interactable=false;
        paginacaoEquipamento[0].interactable=false;        
    }

    public void mascaraClicada(){
        if(naComparacao){
            comparadorGameObject.limparComparador();
            comparadorGameObject.gameObject.SetActive(false);
            mascaraDentroDoHolder.SetActive(false);
            naComparacao=false;
        }
        else{
            esconderEquipamentos();
            mascaraDentroDoHolder.SetActive(false);
            mascaraForaDoHolder.SetActive(false);
        }
    }

    public void mostrarEquipamentos(){
        trocaDeEquipamentosHolder.SetActive(true);
        mascaraForaDoHolder.SetActive(true);
    }

    public void esconderEquipamentos(){
        trocaDeEquipamentosHolder.SetActive(false);
        comparadorGameObject.gameObject.SetActive(false);
    }

    public void proximaPagEquipamentos(){
        paginaEqui++;
        if(paginaEqui==11){//Chegou no final ?
            paginacaoEquipamento[1].interactable=false;
        }else{//Se não, ativar os dois botões
            paginacaoEquipamento[0].interactable=true;
            paginacaoEquipamento[1].interactable=true;
        }
        PopularListaDeEquipamentos();
    }

    public void anteriorPagEquipamentos(){
        paginaEqui--;
        if(paginaEqui==0){//Chegou no começo?
            paginacaoEquipamento[0].interactable=false;
        }else{//Se não, ativar os dois botões
            paginacaoEquipamento[0].interactable=true;
            paginacaoEquipamento[1].interactable=true;
        }
        PopularListaDeEquipamentos();
    }

    public void proximaPagUnidades(){
        paginaUni++;
        if (qualBatalhao == 4){
            if (paginaUni == 9){
                paginacaoUnidade[1].interactable = false;
            }
            else{
                paginacaoUnidade[0].interactable = true;
                paginacaoUnidade[1].interactable = true;
            }
        }
        else{
            paginacaoUnidade[1].interactable = false;
            paginacaoUnidade[0].interactable = true;
        }
        escolherLista();
    }

    public void anteriorPagUnidades(){
        paginaUni--;
        if (qualBatalhao == 4){
            if (paginaUni == 0){
                paginacaoUnidade[0].interactable = false;
            }
            else{
                paginacaoUnidade[1].interactable = true;
                paginacaoUnidade[0].interactable = true;
            }
        }
        else{
            paginacaoUnidade[1].interactable = true;
            paginacaoUnidade[0].interactable = false;
        }
        escolherLista();
    }

    private void escolherLista(){
        switch (qualBatalhao){
            case 1: PopularListaDoPrimeiroBatalhao(); break;
            case 2: PopularListaDoSegundoBatalhao(); break;
            case 3: PopularListaDoTerceiroBatalhao(); break;
            case 4: PopularListaDeNomes(); break;
            default: Debug.Log("Algo deu errado"); break;
        }
    }

    public void PopularListaDeNomes(){
        for (int i = 0; i < 3; i++){
            if ((paginaUni * 3 + i + 1) <= GameController.instancia.reservas.Count && !GameController.instancia.reservas[paginaUni * 3 + i].Equals(null)){
                nomeSelecionaveis[i].GetComponentInChildren<Image>().sprite = BancoDeDados.instancia.visualSecundarioUnidadesDoJogo[GameController.instancia.reservas[paginaUni * 3 + i].idNoBancoDeDados];
                nomeSelecionaveis[i].GetComponent<Button>().interactable = true;
            }
            else{
                nomeSelecionaveis[i].GetComponentInChildren<Image>().sprite = emptySelection;
                nomeSelecionaveis[i].GetComponent<Button>().interactable = false;
            }
            nomeSelecionaveis[i].GetComponent<Outline>().enabled = false;
        }
        paginacaoUnidadeFeedback.text = (paginaUni + 1).ToString();
    }

    public void PopularListaDoPrimeiroBatalhao(){
        for(int i = 0; i < 3; i++){
            if ((paginaUni * 3 + i + 1) <= GameController.instancia.primeiroBatalhao.Count && !GameController.instancia.primeiroBatalhao[i].Equals(null)){
                nomeSelecionaveis[i].GetComponentInChildren<Image>().sprite = BancoDeDados.instancia.visualSecundarioUnidadesDoJogo[GameController.instancia.primeiroBatalhao[i].idNoBancoDeDados];
                nomeSelecionaveis[i].GetComponent<Button>().interactable = true;
            }
            else{
                nomeSelecionaveis[i].GetComponentInChildren<Image>().sprite = emptySelection;
                nomeSelecionaveis[i].GetComponent<Button>().interactable = false;
            }
            nomeSelecionaveis[i].GetComponent<Outline>().enabled = false;
        }
        paginacaoUnidadeFeedback.text = (paginaUni + 1).ToString();
    }

    public void PopularListaDoSegundoBatalhao(){
        for (int i = 0; i < 3; i++){
            if ((paginaUni * 3 + i + 1) <= GameController.instancia.segundoBatalhao.Count && !GameController.instancia.segundoBatalhao[i].Equals(null)){
                nomeSelecionaveis[i].GetComponentInChildren<Image>().sprite = BancoDeDados.instancia.visualSecundarioUnidadesDoJogo[GameController.instancia.segundoBatalhao[i].idNoBancoDeDados];
                nomeSelecionaveis[i].GetComponent<Button>().interactable = true;
            }
            else{
                nomeSelecionaveis[i].GetComponentInChildren<Image>().sprite = emptySelection;
                nomeSelecionaveis[i].GetComponent<Button>().interactable = false;
            }
            nomeSelecionaveis[i].GetComponent<Outline>().enabled = false;
        }
        paginacaoUnidadeFeedback.text = (paginaUni + 1).ToString();
    }

    public void PopularListaDoTerceiroBatalhao(){
        for (int i = 0; i < 3; i++){
            if ((paginaUni * 3 + i + 1) <= GameController.instancia.terceiroBatalhao.Count && !GameController.instancia.terceiroBatalhao[i].Equals(null)){
                nomeSelecionaveis[i].GetComponentInChildren<Image>().sprite = BancoDeDados.instancia.visualSecundarioUnidadesDoJogo[GameController.instancia.terceiroBatalhao[i].idNoBancoDeDados];
                nomeSelecionaveis[i].GetComponent<Button>().interactable = true;
            }
            else{
                nomeSelecionaveis[i].GetComponentInChildren<Image>().sprite = emptySelection;
                nomeSelecionaveis[i].GetComponent<Button>().interactable = false;
            }
            nomeSelecionaveis[i].GetComponent<Outline>().enabled = false;
        }
        paginacaoUnidadeFeedback.text = (paginaUni + 1).ToString();
    }

    public void removerEquipamentoNoSlot(int qual){
        GameController.instancia.equipamentos.Add(ChosenOne.equipamento[qual]);
        GameController.instancia.organizarEquipamentoPorTipo();
        ChosenOne.equipamento.Remove(ChosenOne.equipamento[qual]);
        ChosenOne.calcularStatusComEquipamentoELevel();
        InicializarAmostra();
        PopularListaDeEquipamentos();
    }

    public void PopularListaDeEquipamentos(){
        for (int i = 0; i < 10; i++){
                if ((paginaEqui * 10 + i + 1) <= GameController.instancia.equipamentos.Count && !GameController.instancia.equipamentos[paginaUni * 10 + i].Equals(null)){
                    equipamentosIdle[i].GetComponentInChildren<Text>().text = GameController.instancia.equipamentos[paginaEqui * 10 + i].nome;
                    equipamentosIdle[i].transform.GetChild(1).gameObject.GetComponentInChildren<Image>().sprite = BancoDeDados.instancia.iconesDeTiposDeEquipamentos[(int)GameController.instancia.equipamentos[paginaEqui * 10 + i].tipoDeEquipamento];
                    if(ChosenOne != null && ChosenOne.equipamentosNaoPermitidos.Count > 0 && ChosenOne.equipamentosNaoPermitidos.Contains(GameController.instancia.equipamentos[paginaEqui * 10 + i].tipoDeEquipamento)){
                        equipamentosIdle[i].GetComponent<Button>().interactable = false;
                    }
                    else {
                        equipamentosIdle[i].GetComponent<Button>().interactable = true;
                    }                    
                }
                else{
                    equipamentosIdle[i].GetComponentInChildren<Text>().text = "";
                    equipamentosIdle[i].transform.GetChild(1).gameObject.GetComponentInChildren<Image>().sprite = nenhumaClasseSprite;
                    equipamentosIdle[i].GetComponent<Button>().interactable = false;
                }
            }
            paginacaoEquipamentoFeedback.text = (paginaEqui + 1).ToString();
        }


    public void nullChosenOne(){
        imagenDaUnidade.sprite = CartaVoidUnidade;
        for (int i = 0; i < 4; i++){
            equipamentos[i].GetComponentInChildren<Text>().text = "";
            equipamentosIcones[i].sprite = nenhumaClasseSprite;
            equipamentos[i].GetComponent<Button>().interactable = false;
            equipamentos[i].GetComponent<Button>().spriteState = naoAtribuidoState;
            equipamentos[i].GetComponent<Image>().sprite = naoAtribuidoSprite;
            removerEquipamentos[i].interactable = false;
        }
        for (int i = 0; i < 11; i++){
             status[i].text = "-";
        }

    }

    public void InicializarAmostra(){ //Fazer a primeira pagina de unidades e mostrar a primeira unidade na lista
        imagenDaUnidade.sprite = BancoDeDados.instancia.visualPrincipalUnidadesDoJogo[ChosenOne.idNoBancoDeDados];
        for(int i = 0; i < 4; i++){
            if (i < ChosenOne.equipamento.Count && !ChosenOne.equipamento[i].Equals(null)){
                equipamentos[i].GetComponentInChildren<Text>().text = ChosenOne.equipamento[i].nome;
                equipamentosIcones[i].sprite = BancoDeDados.instancia.iconesDeTiposDeEquipamentos[(int)ChosenOne.equipamento[i].tipoDeEquipamento];
                equipamentos[i].GetComponent<Button>().interactable = true;
                equipamentos[i].GetComponent<Button>().spriteState = atribuidoState;
                equipamentos[i].GetComponent<Image>().sprite = atribuidoSprite;
                removerEquipamentos[i].interactable = true;
            }
            else if(i<= ChosenOne.maximoDeEquipamentos){
                equipamentos[i].GetComponentInChildren<Text>().text = "";
                equipamentosIcones[i].sprite = nenhumaClasseSprite;
                equipamentos[i].GetComponent<Button>().interactable = true;
                equipamentos[i].GetComponent<Button>().spriteState = naoAtribuidoState;
                equipamentos[i].GetComponent<Image>().sprite = naoAtribuidoSprite;
                removerEquipamentos[i].interactable = false;
            }
            else{
                equipamentos[i].GetComponentInChildren<Text>().text = "";
                equipamentosIcones[i].sprite = nenhumaClasseSprite;
                equipamentos[i].GetComponent<Button>().interactable = false;
                removerEquipamentos[i].interactable = false;                
            }
        }
        
        if (ChosenOne.equipamento.Count != 4){
            equipamentos[ChosenOne.equipamento.Count].GetComponent<Button>().interactable = true;
        }

        for(int i = 0; i < 11; i++){
            if (i == 0) { status[i].text = ChosenOne.status[i].ToString(); }
            else {
                status[i].text = ChosenOne.status[i+1].ToString();
            }
        }

        PopularListaDeEquipamentos();
    }   

    //Função serve para impedir o usuario de mudar o slot clicado, gerando bugs no processo, desabilitando os botões até ele escolher o equipamento
    public void DesOuHabilitarTrocaDeEquipamentos(Boolean habilitarOuDesabilitar){
        foreach(GameObject b in equipamentos){
            b.GetComponent<Button>().interactable = habilitarOuDesabilitar;
        }
    }

    public void CliqueNoEquipamento(int qual) {
        slotClicado = qual;
    }

    public void TrocarBatalhao(int qualBat) {
        qualBatalhao = qualBat;
        paginaUni = 0;
        batalhaoVisualFeedback.sprite = batalhaoSelecionado[qualBatalhao - 1];
        escolherLista();

        if (qualBatalhao == 1 && GameController.instancia.primeiroBatalhao.Count == 0) { nullChosenOne(); return; }
        else if (qualBatalhao == 2 && GameController.instancia.segundoBatalhao.Count == 0) { nullChosenOne(); return; }
        else if (qualBatalhao == 3 && GameController.instancia.terceiroBatalhao.Count == 0) { nullChosenOne(); return; }
        else if (qualBatalhao == 4 && GameController.instancia.reservas.Count == 0) { nullChosenOne(); return; }

        TrocarChosenOne(0);//Colocar para apresentar o primeiro do batalhão selecionado
    }

    public void TrocarChosenOne(int qualSlot){
        //Clicou no mesmo slot que já estava ativo, ignorar requisição
        //Ativar o outline no novo slot, e desativar no antigo
        for(int i = 0; i < 3; i++){
            if(qualSlot == i)   nomeSelecionaveis[i].GetComponent<Outline>().enabled = true;
            else                nomeSelecionaveis[i].GetComponent<Outline>().enabled = false;
        }

        switch (qualBatalhao){
            case 1: ChosenOne = GameController.instancia.primeiroBatalhao[paginaUni * 3 + qualSlot]; break;
            case 2: ChosenOne = GameController.instancia.segundoBatalhao[paginaUni * 3 +  qualSlot];  break;
            case 3: ChosenOne = GameController.instancia.terceiroBatalhao[paginaUni * 3 + qualSlot]; break;
            case 4: ChosenOne = GameController.instancia.reservas[paginaUni * 3 + qualSlot];         break;
            default: Debug.Log("Algo deu errado no TrocarChosenOne()"); break;
        }    
        InicializarAmostra();
    }

    public void abrirComparacao(int qualEquipamentoNaReserva){
        equipamentoNoArsenalClicado = qualEquipamentoNaReserva;
        naComparacao=true;
        comparadorGameObject.gameObject.SetActive(true);
        comparadorGameObject.popularEquipamentoParaMudanca(GameController.instancia.equipamentos[((10 * paginaEqui) + qualEquipamentoNaReserva)],ChosenOne,slotClicado);
        mascaraDentroDoHolder.SetActive(true);
    }

    //Esse aqui e meio complicado, por isso tanto comentario
    public void TrocarEquipamento(){
        naComparacao=false;
        mascaraClicada();
        //Verificar se há a nescessidade de substituir algum equipamento 
        //Checar se sequer precisamos checar(Count>0) e depois checar se o slot 
        //clicado e menor que a quantidade atual de equipamentos, ou seja anterior ao fim da list
        if (ChosenOne.equipamento.Count>0 && ChosenOne.equipamento.Count>slotClicado){
            GameController.instancia.equipamentos.Add(ChosenOne.equipamento[slotClicado]);
            ChosenOne.equipamento.Remove(ChosenOne.equipamento[slotClicado]);
            //O insert vai colocar o equipamento no local indicado
            ChosenOne.equipamento.Insert(slotClicado, GameController.instancia.equipamentos[(10 * paginaEqui) + equipamentoNoArsenalClicado]);
        }
        //Else vai colocar o equipamento no final da lista(por exemplo, so tem 1 equipamento e o player clicou no slot 2,3 ou 4)
        else ChosenOne.equipamento.Add(GameController.instancia.equipamentos[(10 * paginaEqui) + equipamentoNoArsenalClicado]);
        
        //ChosenOne.equipamento[slotClicado] = GameController.instancia.equipamentos[(10 * paginaEqui) + qualSlot2];
        GameController.instancia.equipamentos.Remove(GameController.instancia.equipamentos[(10 * paginaEqui) + equipamentoNoArsenalClicado]);
        GameController.instancia.organizarEquipamentoPorTipo();
        ChosenOne.calcularStatusComEquipamentoELevel();
        PopularListaDeEquipamentos();//Como a lista de equipamentos foi mechida(1 foi removido e possivelmente 1 foi adicionado), refaze-la
        InicializarAmostra();//Atualizar a amostragem dos equipamentos
    }
}
