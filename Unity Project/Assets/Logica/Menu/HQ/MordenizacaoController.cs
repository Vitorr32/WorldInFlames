using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Logica.Classes;
using System.Linq;

public class MordenizacaoController : MonoBehaviour {
    public Text combustivel;
    public Text metal;
    public Text inpsiracao;
    public Text mdo;
    public Text municao;

    int custoCombustivel = 0, custoMunicao = 0, custoMetal = 0, custoMDO = 0, custoInspiracao = 0;

    public GameObject trocaHolder;
    public GameObject mascara;
    public GameObject upToDate;

    public Text paginacaoUnidadeFeedback;
    public Button[] paginacaoUnidade;
    public GameObject[] unidadesIdle;
    public GameObject paginaDeUnidades;
    public GameObject paginacadoUnidade;

    private int paginaUni = 0;
    private Unidade unidadeAtual;
    private Unidade unidadeMordenizada;

    public CartaUnidadeMordenizacao cartaDaUnidadeAtual;
    public CartaUnidadeMordenizacao cartaDaUnidadeMordenizada;

    public Button trocar;
    public Button recrutar;

    private List<Unidade> unidadesDoPlayer;

    void Awake(){
        popularUnidadeNaListaDaMordenizacao(true);
        atualizarInterfaceInteira();
    }

    public void popularUnidadeNaListaDaMordenizacao(bool semUnidadeAtual){
        unidadesDoPlayer = new List<Unidade>();
        if (GameController.instancia.primeiroBatalhao.Count != 0) unidadesDoPlayer.AddRange(GameController.instancia.primeiroBatalhao);
        if (GameController.instancia.segundoBatalhao.Count != 0) unidadesDoPlayer.AddRange(GameController.instancia.segundoBatalhao);
        if (GameController.instancia.terceiroBatalhao.Count != 0) unidadesDoPlayer.AddRange(GameController.instancia.terceiroBatalhao);
        if (GameController.instancia.reservas.Count != 0) unidadesDoPlayer.AddRange(GameController.instancia.reservas);
        if(semUnidadeAtual) unidadeAtual = unidadesDoPlayer[0];
    }

    public void mostrarPaginaDeUnidades(){
        mascara.SetActive(true);
        paginacadoUnidade.SetActive(true);
        paginaDeUnidades.SetActive(true);
        trocaHolder.SetActive(true);
    }

    public void esconderPaginaDeUnidades(){
        paginaUni = 0;
        mascara.SetActive(false);
        paginacadoUnidade.SetActive(false);
        paginaDeUnidades.SetActive(false);
        trocaHolder.SetActive(false);
    }

    public void mascaraClicada(){
        esconderPaginaDeUnidades();
    }

    public void proximaPagUnidades(){
        paginaUni++;
        if (paginaUni == 2){
            paginacaoUnidade[1].interactable = false;
        }
        else if (paginaUni == 5){
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

    private void atualizarInterfaceInteira(){
        atualizarUnidadeAtual();
        if (unidadeMordenizada != null) { verificarPossibilidadeDeMordenizacao(); }
        else { unidadeMordenizadaNula(); recrutar.interactable = false; }
        PopularListaDeNomes();
        if (unidadesDoPlayer.Count < 2 && recrutar.interactable) { trocar.interactable = false; }
        else { trocar.interactable = true;}
    }

    private void verificarPossibilidadeDeMordenizacao(){
        Pais paisDoAlvo = unidadeMordenizada.nacionalidade;
        Nacoes desenvolvimentoAlvo = GameController.instancia.nacoesDoJogador.Find(d => paisDoAlvo == d.pais);

        if (calcularCustoDeMordenizacao() //Tem recursos o suficiente (Também atualiza a interface por isso vem primeiro)
            && unidadeAtual.lvl > unidadeAtual.upgradeLvl //Está no nivel minimo
            && desenvolvimentoAlvo.nivel >= unidadeMordenizada.desenvolvimentoRequerido //Tem tecnologia o suficiente
            && !unidadeMordenizada.Equals(null)){// Não chegou ao utlimo estagio de mordenização, não há mais o que atualizar
            recrutar.interactable = true;
        }
        else{
            recrutar.interactable = false;
        }
    }

    public void trocarUnidadeAtual(int qual){
        unidadeAtual = unidadesDoPlayer[paginaUni * 10 + qual];
        atualizarInterfaceInteira();
    }

    public void atualizarUnidadeAtual(){
        cartaDaUnidadeAtual.popularCartaDeModernizacao(unidadeAtual,false);
        if (unidadeAtual.idDaMordenizacao == -1) { unidadeMordenizada = null; upToDate.SetActive(true); return; }        
        else {
            upToDate.SetActive(false);
            this.unidadeMordenizada = BancoDeDados.instancia.retornarUnidade(unidadeAtual.idDaMordenizacao);
            cartaDaUnidadeMordenizada.popularCartaDeModernizacao(unidadeMordenizada, true);
        }
    }

    private void PopularListaDeNomes(){
        for (int i = 0; i < 10; i++){
            if ((paginaUni * 10 + i + 1) <= unidadesDoPlayer.Count) {
                unidadesIdle[i].transform.GetChild(1).GetComponent<Image>().sprite = BancoDeDados.instancia.iconesDasClasses[(int)unidadesDoPlayer[paginaUni * 10 + i].classe];
                unidadesIdle[i].GetComponentInChildren<Text>().text = unidadesDoPlayer[paginaUni * 10 + i].nome;
                unidadesIdle[i].GetComponent<Button>().interactable = true;
            }
            else{
                unidadesIdle[i].transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Graficos/Texturas/Imagens/Classes/NoClass");
                unidadesIdle[i].GetComponentInChildren<Text>().text = "";
                unidadesIdle[i].GetComponent<Button>().interactable = false;
            }
        }
        paginacaoUnidadeFeedback.text = (paginaUni + 1).ToString();
    }

    public void MordenizarUnidade(){
        //Retirar recursos do jogador
        GameController.instancia.Combustivel -= (int)custoCombustivel;
        GameController.instancia.Metal -= (int)custoMetal;
        GameController.instancia.Municao -= (int)custoMunicao;
        GameController.instancia.MaoDeObra -= (int)custoMDO;
        GameController.instancia.Inspiracao -= (int)custoInspiracao;
        EventosDoHQ.instancia.atualizarInterface();
        //Limpar qualquer traço da unidade pre-mordenizada
        if (GameController.instancia.primeiroBatalhao.Contains(unidadeAtual)) { GameController.instancia.primeiroBatalhao.Remove(unidadeAtual); }
        else if (GameController.instancia.segundoBatalhao.Contains(unidadeAtual)) { GameController.instancia.segundoBatalhao.Remove(unidadeAtual); }
        else if (GameController.instancia.terceiroBatalhao.Contains(unidadeAtual)) { GameController.instancia.terceiroBatalhao.Remove(unidadeAtual); }
        else if (GameController.instancia.reservas.Contains(unidadeAtual)) { GameController.instancia.reservas.Remove(unidadeAtual); }
        //Comecar mordenizacao, retirando os equipamentos da unidade 
        foreach(Equipamento e in unidadeAtual.equipamento){
            GameController.instancia.equipamentos.Add(e);            
        }
        //unidadeAtual.equipamento = new List<Equipamento>();
        unidadeMordenizada.lvl = unidadeAtual.lvl;
        unidadeMordenizada.idDaUnidade = unidadeAtual.idDaUnidade;
        unidadeMordenizada.experienciaAtual = unidadeAtual.experienciaAtual;
        //Adicionar a nova unidade aos reservas
        GameController.instancia.reservas.Add(unidadeMordenizada);
        //Fazer a unidade atual ser a nova unidade para visual
        unidadeAtual = unidadeMordenizada;
        unidadeMordenizada = null;

        popularUnidadeNaListaDaMordenizacao(false);
        atualizarInterfaceInteira();
    }

    private void unidadeMordenizadaNula(){
        combustivel.text = "-";
        municao.text = "-";
        metal.text = "-";
        mdo.text = "-";
        inpsiracao.text = "-";
    }

    private bool calcularCustoDeMordenizacao(){
        switch (unidadeMordenizada.classe){
            case Classe.Infantaria: custoCombustivel = 50; custoMunicao = 200; custoMetal = 200; custoMDO = 100; custoInspiracao = 1; break;
            case Classe.InfantariaNaval: custoCombustivel = 50; custoMunicao = 200; custoMetal = 200; custoMDO = 100; custoInspiracao = 5; break;
            case Classe.InfantariaAlpina: custoCombustivel = 50; custoMunicao = 200; custoMetal = 200; custoMDO = 100; custoInspiracao = 5; break;
            case Classe.Artilharia: custoCombustivel = 50; custoMunicao = 300; custoMetal = 300; custoMDO = 10; custoInspiracao = 1; break;
            case Classe.AntiTank: custoCombustivel = 50; custoMunicao = 300; custoMetal = 300; custoMDO = 10; custoInspiracao = 1; break;
            case Classe.RocketArtilharia: custoCombustivel = 50; custoMunicao = 300; custoMetal = 300; custoMDO = 10; custoInspiracao = 5; break;
            case Classe.Cavalaria: custoCombustivel = 100; custoMunicao = 200; custoMetal = 200; custoMDO = 150; custoInspiracao = 1; break;
            case Classe.FullMecanizada: custoCombustivel = 400; custoMunicao = 400; custoMetal = 400; custoMDO = 250; custoInspiracao = 10; break;
            case Classe.FullMotorizada: custoCombustivel = 300; custoMunicao = 300; custoMetal = 300; custoMDO = 150; custoInspiracao = 5; break;
            case Classe.SemiMecanizada: custoCombustivel = 350; custoMunicao = 350; custoMetal = 350; custoMDO = 200; custoInspiracao = 5; break;
            case Classe.SemiMotorizada: custoCombustivel = 250; custoMunicao = 250; custoMetal = 250; custoMDO = 100; custoInspiracao = 3; break;
            case Classe.TankDestroyer: custoCombustivel = 400; custoMunicao = 500; custoMetal = 500; custoMDO = 30; custoInspiracao = 10; break;
            case Classe.SPArtilharia: custoCombustivel = 300; custoMunicao = 400; custoMetal = 400; custoMDO = 20; custoInspiracao = 5; break;
            case Classe.SPRocketArtilharia: custoCombustivel = 400; custoMunicao = 500; custoMetal = 500; custoMDO = 30; custoInspiracao = 10; break;
            case Classe.TanqueLeve: custoCombustivel = 400; custoMunicao = 400; custoMetal = 400; custoMDO = 100; custoInspiracao = 1; break;
            case Classe.TanqueMedio: custoCombustivel = 500; custoMunicao = 600; custoMetal = 600; custoMDO = 150; custoInspiracao = 5; break;
            case Classe.TanquePesado: custoCombustivel = 700; custoMunicao = 800; custoMetal = 800; custoMDO = 200; custoInspiracao = 10; break;
            case Classe.TanqueUltraPesado: custoCombustivel = 400; custoMunicao = 1000; custoMetal = 1000; custoMDO = 50; custoInspiracao = 10; break;
        }
        Nacoes nacaoDaUnidadeMordenizada = GameController.instancia.nacoesDoJogador.Find(dens => dens.pais == unidadeMordenizada.nacionalidade);
        custoCombustivel = (int)(custoCombustivel * nacaoDaUnidadeMordenizada.modfCombustivel);
        custoMunicao = (int)(custoMunicao * nacaoDaUnidadeMordenizada.modfMunicao);
        custoMetal = (int)(custoMetal * nacaoDaUnidadeMordenizada.modfMetal);
        custoMDO = (int)(custoMDO * nacaoDaUnidadeMordenizada.modfMaoDeObra);
        custoInspiracao = (int)(custoInspiracao * nacaoDaUnidadeMordenizada.modfInspiracao);

        combustivel.text = custoCombustivel.ToString();
        municao.text = custoMunicao.ToString();
        metal.text = custoMetal.ToString();
        mdo.text = custoMDO.ToString();
        inpsiracao.text = custoInspiracao.ToString();

        if (custoCombustivel > GameController.instancia.Combustivel || custoMetal > GameController.instancia.Metal ||
            custoMunicao > GameController.instancia.Municao || custoMDO > GameController.instancia.MaoDeObra ||
            custoInspiracao > GameController.instancia.Inspiracao) {//Checar se o player tem recursos o suficiente
            return false;
        }
        return true;
    }
}
