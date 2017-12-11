using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class QuartelController : MonoBehaviour {

    public GameObject cartaDeUnidadeQuartelPrefab;
    public GameObject cartaDoVazioQuartePrefab;

    public GameObject unidadeHolderTopo;
    public GameObject unidadeHolderBaixo;
	public GameObject mascaraDeOpcoes;
    
    public GameObject[] slots = new GameObject[6];

    public Image selecaoDeBatalhao;
    public Sprite[] selecaoDeBatalhaoSprites = new Sprite[3];
    public Sprite UnidadeSemClasseSprite;

    public Text paginacaoUnidadeFeedback;
    public Button[] paginacaoUnidade;
    public GameObject[] unidadesIdle;
    public GameObject paginaDeUnidades;
    public GameObject paginacadoUnidade;

    public Button recarregar;

    public Text combustivelCusto;
    public Text municaoCusto;
    public Text mdoCusto;

    int custoTotalCombustivel = 0;
    int custoTotalMunicao = 0;
    int custoTotalMaoDeObra = 0;

    public bool totalDeCusto = false;//Se o custo de recursos mostrado e o total de todas as tropas ou apenas do batalhão atual
    private int paginaUni;
    private int batalhaoAtual;
    private int slotClicado;

    public void FirstEnterQuartel() {
        batalhaoAtual = 1;
        popularBatalhoes();
        PopularListaDeNomes();
        paginaUni = 0;
        paginacaoUnidade[0].interactable = false;
        calcularCustosDeRecarga(false);
    }

    public void mostrarPaginaDeUnidades(){
		mascaraDeOpcoes.SetActive (true);
        paginacadoUnidade.SetActive(true);
        paginaDeUnidades.SetActive(true);
    }

    public void esconderPaginaDeUnidades(){
		paginaUni = 0;
		mascaraDeOpcoes.SetActive (false);
        paginacadoUnidade.SetActive(false);
        paginaDeUnidades.SetActive(false);
    }

    public void MundandoBatalhao(int qual){
        batalhaoAtual = qual;
        selecaoDeBatalhao.sprite = selecaoDeBatalhaoSprites[batalhaoAtual - 1];
        popularBatalhoes();
    }

    public void SlotClicado(int slot){
        slotClicado = slot;
    }

	public void mascaraClicada(){
		esconderPaginaDeUnidades ();
	}

    public void proximaPagUnidades(){
        paginaUni++;
        if (paginaUni == 2){
            paginacaoUnidade[1].interactable = false;
        }
        else if(paginaUni == 5){
            paginacaoUnidade[0].interactable = false;
        }
        else{
            paginacaoUnidade[0].interactable = true;
            paginacaoUnidade[1].interactable = true;
        }
        PopularListaDeNomes();
    }

    public void anteriorPagUnidades(){
        paginaUni--;
        if (paginaUni == 0){
            paginacaoUnidade[0].interactable = false;
        }
        else{
            paginacaoUnidade[1].interactable = true;
            paginacaoUnidade[0].interactable = true;
        }
        PopularListaDeNomes();
    }

    void destruirSlots(){
        for(int i = slots.Length-1; i >=0; i--){
            if(slots[i]!=null) {Destroy(slots[i].gameObject); }               
        }
    }    

    public void calcularCustosDeRecarga(bool total){
        totalDeCusto = total;
        custoTotalCombustivel = 0;custoTotalMaoDeObra = 0;custoTotalMunicao = 0;//Resetar custos
        if (total) {
            foreach (Unidade u in GameController.instancia.primeiroBatalhao) {
                if (u.status[13] < 100 || u.status[15] < 100 || u.status[1] != u.status[0]) {//A unidade não tem tudo que precisa
                    if (u.status[13] < 100) { custoTotalCombustivel += (u.status[12] * (100 - u.status[13])) / 100; }
                    if (u.status[15] < 100) { custoTotalMunicao += (u.status[14] * (100 - u.status[15])) / 100; }
                    if (u.status[1] != 0) { custoTotalMaoDeObra += u.status[16] - (((u.status[1] * 100) / u.status[0]) * u.status[16]) / 100; }
                }
            }
            foreach (Unidade u in GameController.instancia.segundoBatalhao) {
                if (u.status[13] < 100 || u.status[15] < 100 || u.status[1] != u.status[0]) {//A unidade não tem tudo que precisa
                    if (u.status[13] < 100) { custoTotalCombustivel += (u.status[12] * (100 - u.status[13])) / 100; }
                    if (u.status[15] < 100) { custoTotalMunicao += (u.status[14] * (100 - u.status[15])) / 100; }
                    if (u.status[1] != 0) { custoTotalMaoDeObra += u.status[16] - (((u.status[1] * 100) / u.status[0]) * u.status[16]) / 100; }
                }
            }
            foreach (Unidade u in GameController.instancia.terceiroBatalhao) {
                if (u.status[13] < 100 || u.status[15] < 100 || u.status[1] != u.status[0]) {//A unidade não tem tudo que precisa
                    if (u.status[13] < 100) { custoTotalCombustivel += (u.status[12] * (100 - u.status[13])) / 100; }
                    if (u.status[15] < 100) { custoTotalMunicao += (u.status[14] * (100 - u.status[15])) / 100; }
                    if (u.status[1] != 0) { custoTotalMaoDeObra += u.status[16] - (((u.status[1] * 100) / u.status[0]) * u.status[16]) / 100; }
                }
            }
        }
        else {
            switch (batalhaoAtual) {
                case 1:
                    foreach (Unidade u in GameController.instancia.primeiroBatalhao) {
                        if (u.status[13] < 100 || u.status[15] < 100 || u.status[1] != u.status[0]) {//A unidade não tem tudo que precisa
                            if (u.status[13] < 100) { custoTotalCombustivel += (u.status[12] * (100 - u.status[13])) / 100; }
                            if (u.status[15] < 100) { custoTotalMunicao += (u.status[14] * (100 - u.status[15])) / 100; }
                            if (u.status[1] != 0) { custoTotalMaoDeObra += u.status[16] - (((u.status[1] * 100) / u.status[0]) * u.status[16]) / 100; }
                        }
                    }
                    break;
                case 2:
                    foreach (Unidade u in GameController.instancia.segundoBatalhao) {
                        if (u.status[13] < 100 || u.status[15] < 100 || u.status[1] != u.status[0]) {//A unidade não tem tudo que precisa
                            if (u.status[13] < 100) { custoTotalCombustivel += (u.status[12] * (100 - u.status[13])) / 100; }
                            if (u.status[15] < 100) { custoTotalMunicao += (u.status[14] * (100 - u.status[15])) / 100; }
                            if (u.status[1] != 0) { custoTotalMaoDeObra += u.status[16] - (((u.status[1] * 100) / u.status[0]) * u.status[16]) / 100; }
                        }
                    }
                    break;
                case 3:
                    foreach (Unidade u in GameController.instancia.terceiroBatalhao) {
                        if (u.status[13] < 100 || u.status[15] < 100 || u.status[1] != u.status[0]) {//A unidade não tem tudo que precisa
                            if (u.status[13] < 100) { custoTotalCombustivel += (u.status[12] * (100 - u.status[13])) / 100; }
                            if (u.status[15] < 100) { custoTotalMunicao += (u.status[14] * (100 - u.status[15])) / 100; }
                            if (u.status[1] != 0) { custoTotalMaoDeObra += u.status[16] - (((u.status[1] * 100) / u.status[0]) * u.status[16]) / 100; }
                        }
                    }
                    break;
            }
        }
        verificarRecursoseBloquearRecarga();
        apresentarCustosNaInterface();        
    }

    public void verificarRecursoseBloquearRecarga(){
        if(custoTotalCombustivel > GameController.instancia.Combustivel || 
            custoTotalMaoDeObra> GameController.instancia.MaoDeObra || custoTotalMunicao > GameController.instancia.Municao){
            recarregar.interactable = false;
        }else { recarregar.interactable = true; }
    }

    public void efetivarRecarga() {
        GameController.instancia.Combustivel -= custoTotalCombustivel;
        GameController.instancia.Municao -= custoTotalMunicao;
        GameController.instancia.MaoDeObra -= custoTotalMaoDeObra;
        if (totalDeCusto){
            foreach (Unidade u in GameController.instancia.primeiroBatalhao){
                if (u.status[13] != 100) { u.status[13] = 100; }
                if (u.status[15] != 100) { u.status[15] = 100; }
                if (u.status[1] != u.status[0]) { u.status[1] = u.status[0]; }
            }
            foreach (Unidade u in GameController.instancia.segundoBatalhao){
                if (u.status[13] != 100) { u.status[13] = 100; }
                if (u.status[15] != 100) { u.status[15] = 100; }
                if (u.status[1] != u.status[0]) { u.status[1] = u.status[0]; }
            }
            foreach (Unidade u in GameController.instancia.terceiroBatalhao){
                if (u.status[13] != 100) { u.status[13] = 100; }
                if (u.status[15] != 100) { u.status[15] = 100; }
                if (u.status[1] != u.status[0]) { u.status[1] = u.status[0]; }
            }
        }
        else{
            switch (batalhaoAtual){
                case 1:
                    foreach (Unidade u in GameController.instancia.primeiroBatalhao){
                        if (u.status[13] != 100) { u.status[13] = 100; }
                        if (u.status[15] != 100) { u.status[15] = 100; }
                        if (u.status[1] != u.status[0]) { u.status[1] = u.status[0]; }
                    }
                    break;
                case 2:
                    foreach (Unidade u in GameController.instancia.segundoBatalhao){
                        if (u.status[13] != 100) { u.status[13] = 100; }
                        if (u.status[15] != 100) { u.status[15] = 100; }
                        if (u.status[1] != u.status[0]) { u.status[1] = u.status[0]; }
                    }
                    break;
                case 3:
                    foreach (Unidade u in GameController.instancia.terceiroBatalhao){
                        if (u.status[13] != 100) { u.status[13] = 100; }
                        if (u.status[15] != 100) { u.status[15] = 100; }
                        if (u.status[1] != u.status[0]) { u.status[1] = u.status[0]; }
                    }
                    break;
            }
        }
        custoTotalCombustivel = 0;
        custoTotalMunicao = 0;
        custoTotalMaoDeObra = 0;
        totalDeCusto = false;
        calcularCustosDeRecarga(false);
        popularBatalhoes();
    }

    public void apresentarCustosNaInterface(){
        combustivelCusto.text = custoTotalCombustivel.ToString();
        if (custoTotalCombustivel != 0) { combustivelCusto.color = Color.red; }
        else { combustivelCusto.color = Color.green; }
        municaoCusto.text = custoTotalMunicao.ToString();
        if (custoTotalMunicao != 0) { municaoCusto.color = Color.red; }
        else { municaoCusto.color = Color.green; }
        mdoCusto.text = custoTotalMaoDeObra.ToString();
        if (custoTotalMaoDeObra != 0) { mdoCusto.color = Color.red; }
        else { mdoCusto.color = Color.green; }

        if (custoTotalCombustivel == 0 && custoTotalMunicao == 0 && custoTotalMaoDeObra ==0){recarregar.interactable = false;}
        else { recarregar.interactable = true; }
    }

    public void retirarDoBatalhao(int qual){
        switch(batalhaoAtual) {
            case 1:
                GameController.instancia.reservas.Add(GameController.instancia.primeiroBatalhao[qual]);
                GameController.instancia.primeiroBatalhao.Remove(GameController.instancia.primeiroBatalhao[qual]);
                break;
            case 2:
                GameController.instancia.reservas.Add(GameController.instancia.segundoBatalhao[qual]);
                GameController.instancia.segundoBatalhao.Remove(GameController.instancia.segundoBatalhao[qual]);
                break;
            case 3:
                GameController.instancia.reservas.Add(GameController.instancia.terceiroBatalhao[qual]);
                GameController.instancia.terceiroBatalhao.Remove(GameController.instancia.terceiroBatalhao[qual]);
                break;
        }
        popularBatalhoes();
        PopularListaDeNomes();
    }

    void adicionarEventTriggerDeSFX(GameObject target){
        EventTrigger triggerDeSons = target.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry hover = new EventTrigger.Entry();
        hover.eventID = EventTriggerType.PointerEnter;
        hover.callback.AddListener((eventdata) => { EventosDoHQ.instancia.HoverDeBotaoMetalico(); });
        triggerDeSons.triggers.Add(hover);

        EventTrigger.Entry click = new EventTrigger.Entry();
        click.eventID = EventTriggerType.PointerClick;
        click.callback.AddListener((eventdata) => { EventosDoHQ.instancia.ClickDeBotaoMetalico(); });
        triggerDeSons.triggers.Add(click);
    }
    
    void popularBatalhoes(){   
        destruirSlots();
        switch (batalhaoAtual){
            case 1:
                for (int i = 0; i < 6; i++){
                    int interacaoAtual = i;
                    if (i + 1 <= GameController.instancia.primeiroBatalhao.Count){
                        CartaUnidadeQuartel unidadeNova = Instantiate(cartaDeUnidadeQuartelPrefab).GetComponent<CartaUnidadeQuartel>();
                        slots[i] = unidadeNova.gameObject;
                        if (i<3) { unidadeNova.transform.SetParent(unidadeHolderTopo.transform,false); }
                        else {unidadeNova.transform.SetParent(unidadeHolderBaixo.transform,false); }
                        unidadeNova.PopularCartaUnidadeQuartel(GameController.instancia.primeiroBatalhao[i]);
                        unidadeNova.botaoDeMandarPraReserva.onClick.AddListener(()=>retirarDoBatalhao(interacaoAtual));
                        unidadeNova.botaoDeTroca.onClick.AddListener(()=>mostrarPaginaDeUnidades());
                        unidadeNova.botaoDeTroca.onClick.AddListener(()=>SlotClicado(interacaoAtual));

                        adicionarEventTriggerDeSFX(unidadeNova.botaoDeMandarPraReserva.gameObject);
                        adicionarEventTriggerDeSFX(unidadeNova.botaoDeTroca.gameObject);
                    }
                    else{
                        GameObject cartaDeNaoAtribuido = Instantiate(cartaDoVazioQuartePrefab);
                        adicionarEventTriggerDeSFX(cartaDeNaoAtribuido);
                        if (i<3) { cartaDeNaoAtribuido.transform.SetParent(unidadeHolderTopo.transform,false); }
                        else {cartaDeNaoAtribuido.transform.SetParent(unidadeHolderBaixo.transform,false); }
                        cartaDeNaoAtribuido.GetComponent<Button>().onClick.AddListener(()=>mostrarPaginaDeUnidades());
                        cartaDeNaoAtribuido.GetComponent<Button>().onClick.AddListener(()=>SlotClicado(interacaoAtual));
                        slots[i] = cartaDeNaoAtribuido;
                    }
                }
                if (GameController.instancia.primeiroBatalhao.Count != 6){
                    slots[GameController.instancia.primeiroBatalhao.Count].GetComponentInChildren<Button>().interactable = true;
                }
                break;
            case 2:
                for (int i = 0; i < 6; i++){
                    int interacaoAtual = i;
                    if (i + 1 <= GameController.instancia.segundoBatalhao.Count){
                        CartaUnidadeQuartel unidadeNova = Instantiate(cartaDeUnidadeQuartelPrefab).GetComponent<CartaUnidadeQuartel>();
                        slots[i] = unidadeNova.gameObject;
                        if (i < 3) { unidadeNova.transform.SetParent(unidadeHolderTopo.transform, false); }
                        else { unidadeNova.transform.SetParent(unidadeHolderBaixo.transform, false); }
                        unidadeNova.PopularCartaUnidadeQuartel(GameController.instancia.segundoBatalhao[i]);
                        unidadeNova.botaoDeMandarPraReserva.onClick.AddListener(() => retirarDoBatalhao(interacaoAtual));
                        unidadeNova.botaoDeTroca.onClick.AddListener(() => mostrarPaginaDeUnidades());
                        unidadeNova.botaoDeTroca.onClick.AddListener(() => SlotClicado(interacaoAtual));
                    }
                    else{
                        GameObject cartaDeNaoAtribuido = Instantiate(cartaDoVazioQuartePrefab);
                        if (i < 3) { cartaDeNaoAtribuido.transform.SetParent(unidadeHolderTopo.transform, false); }
                        else { cartaDeNaoAtribuido.transform.SetParent(unidadeHolderBaixo.transform, false); }
                        cartaDeNaoAtribuido.GetComponentInChildren<Button>().onClick.AddListener(() => mostrarPaginaDeUnidades());
                        cartaDeNaoAtribuido.GetComponentInChildren<Button>().onClick.AddListener(() => SlotClicado(interacaoAtual));
                        slots[i] = cartaDeNaoAtribuido;
                    }
                }
                if (GameController.instancia.segundoBatalhao.Count != 6){
                    slots[GameController.instancia.segundoBatalhao.Count].GetComponentInChildren<Button>().interactable = true;
                }
                break;
            case 3:
                for (int i = 0; i < 6; i++){
                    int interacaoAtual = i;
                    if (i + 1 <= GameController.instancia.terceiroBatalhao.Count){
                        CartaUnidadeQuartel unidadeNova = Instantiate(cartaDeUnidadeQuartelPrefab).GetComponent<CartaUnidadeQuartel>();
                        slots[i] = unidadeNova.gameObject;
                        if (i < 3) { unidadeNova.transform.SetParent(unidadeHolderTopo.transform, false); }
                        else { unidadeNova.transform.SetParent(unidadeHolderBaixo.transform, false); }
                        unidadeNova.PopularCartaUnidadeQuartel(GameController.instancia.terceiroBatalhao[i]);
                        unidadeNova.botaoDeMandarPraReserva.onClick.AddListener(() => retirarDoBatalhao(interacaoAtual));
                        unidadeNova.botaoDeTroca.onClick.AddListener(() => mostrarPaginaDeUnidades());
                        unidadeNova.botaoDeTroca.onClick.AddListener(() => SlotClicado(interacaoAtual));
                    }
                    else{
                        GameObject cartaDeNaoAtribuido = Instantiate(cartaDoVazioQuartePrefab);
                        if (i < 3) { cartaDeNaoAtribuido.transform.SetParent(unidadeHolderTopo.transform, false); }
                        else { cartaDeNaoAtribuido.transform.SetParent(unidadeHolderBaixo.transform, false); }
                        cartaDeNaoAtribuido.GetComponentInChildren<Button>().onClick.AddListener(() => mostrarPaginaDeUnidades());
                        cartaDeNaoAtribuido.GetComponentInChildren<Button>().onClick.AddListener(() => SlotClicado(interacaoAtual));
                        slots[i] = cartaDeNaoAtribuido;
                    }
                }
                if (GameController.instancia.terceiroBatalhao.Count != 6){
                    slots[GameController.instancia.terceiroBatalhao.Count].GetComponentInChildren<Button>().interactable = true;
                }
                break;
        }
        
    }
                

    public void TrocarUnidades(int aTrocar){
        switch (batalhaoAtual){
            case 1:
                //Por exemplo, se o player clicar no slot 6 mas so existirem 5 unidades, obviamente o slot 6 está vazio, porque é uma lista
                if (GameController.instancia.primeiroBatalhao.Count > slotClicado){
                    //Retirar o que ja estava no batalhao e colocar na lista de unidades não atribuidas a um batalhão
                    GameController.instancia.reservas.Add(GameController.instancia.primeiroBatalhao[slotClicado]);
                    GameController.instancia.primeiroBatalhao.Remove(GameController.instancia.primeiroBatalhao[slotClicado]);
                }
                //Colocar a nova unidade no batalhão e tirar ela da lista de unidades não atribuidas
			    GameController.instancia.primeiroBatalhao.Insert(slotClicado,GameController.instancia.reservas[paginaUni * 10 + aTrocar]);
                break;
            case 2:
                if (GameController.instancia.segundoBatalhao.Count > slotClicado){
                    //Retirar o que ja estava no batalhao e colocar na lista de unidades não atribuidas a um batalhão
                    GameController.instancia.reservas.Add(GameController.instancia.segundoBatalhao[slotClicado]);
                    GameController.instancia.segundoBatalhao.Remove(GameController.instancia.segundoBatalhao[slotClicado]);
                }
                //Colocar a nova unidade no batalhão e tirar ela da lista de unidades não atribuidas
				GameController.instancia.segundoBatalhao.Insert(slotClicado, GameController.instancia.reservas[paginaUni * 10 + aTrocar]);
                break;
            case 3:
                if (GameController.instancia.terceiroBatalhao.Count > slotClicado){
                    //Retirar o que ja estava no batalhao e colocar na lista de unidades não atribuidas a um batalhão
                    GameController.instancia.reservas.Add(GameController.instancia.terceiroBatalhao[slotClicado]);
                    GameController.instancia.terceiroBatalhao.Remove(GameController.instancia.terceiroBatalhao[slotClicado]);
                }
                //Colocar a nova unidade no batalhão e tirar ela da lista de unidades não atribuidas
				GameController.instancia.terceiroBatalhao.Insert(slotClicado, GameController.instancia.reservas[paginaUni * 10 + aTrocar]);
                break;
        }
        //Finalmente, remover a unidades adicionada ao batalhão do pool de unidades não atribuidas
        GameController.instancia.reservas.Remove(GameController.instancia.reservas[paginaUni * 10 + aTrocar]);
        PopularListaDeNomes();
        popularBatalhoes();        
    }

    public void PopularListaDeNomes(){
        for(int i = 0; i < 10; i++){
            if ((paginaUni*10+i+1) <= GameController.instancia.reservas.Count && !GameController.instancia.reservas[paginaUni * 10 + i].Equals(null)) {
                unidadesIdle[i].transform.GetChild(1).GetComponentInChildren<Image>().sprite= BancoDeDados.instancia.iconesDasClasses[(int)GameController.instancia.reservas[paginaUni*10 + i].classe] ;
                unidadesIdle[i].GetComponentInChildren<Text>().text = GameController.instancia.reservas[paginaUni * 10 + i].nome;
                unidadesIdle[i].GetComponent<Button>().interactable = true;
            }
            else {
                unidadesIdle[i].transform.GetChild(1).GetComponentInChildren<Image>().sprite = UnidadeSemClasseSprite;
                unidadesIdle[i].GetComponentInChildren<Text>().text = "";
                unidadesIdle[i].GetComponent<Button>().interactable = false;
            }
        }
        paginacaoUnidadeFeedback.text = (paginaUni + 1).ToString();
    }
}
