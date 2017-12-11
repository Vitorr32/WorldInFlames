using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Logica.Classes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class CampanhasController : MonoBehaviour {
    public Text descricao;
    public GameObject detalhes;
    public GameObject mascaraDeOpcoes;
    public GameObject campanhasBotoesHolder;
    public Image imagemDaCampanha;
    public Image[] amostragem;
    public Text objetivos;
    public Button startCampanha;
    public GameObject botaoCampanhaPrefab;

    public ScrollRect conteudoDeScroll;
    public Scrollbar scrollDaDescricao;
    public Slider sliderDePosicaoDoConteudo;

    public Button[] campanhas;
    public Sprite[] imagensDeDemonstracaoDaCampanha;

    public Text objetivosDaCampanha;
    public Text restricoesDaCampanha;
    public Text feedbackDasRestricoes;

    public Sprite NoArmyAssigned;
    public Sprite[] batalhaoSelecionadoSprites;

    public Image selecaoDoBatalhao;
    public Image nomeDaNacaoDaCampanha;
    public Image iconeDaNacaoDaCampanha;

    private int campanhaSelecionada;
    private int batalhaoSelecionado;

    void Start(){
        AtivarCampanhas();
        mudarCampanhaSelecionada(BancoDeDados.instancia.campanhasDoJogo.FindIndex(x => x.ativo));
        detalhes.SetActive(false);
        batalhaoSelecionado = 1;
        campanhaSelecionada = 0;
        conteudoDeScroll.verticalNormalizedPosition = 1;
        scrollDaDescricao.value = 1;
    }

    public void SiegHeil(){
        mascaraDeOpcoes.SetActive(true);
        detalhes.SetActive(true);
        configurarRestricoesEObjetivos();
        verificarRestricoes();
        PopularAmostragem();        
    }

    public void listarClassesProibidas(List<Classe> classes){
        restricoesDaCampanha.text += "As seguintes classes estão proibidas : ";
        foreach(Classe classe in classes){
            associarNomeParaClasse(classe, restricoesDaCampanha);
            if (classe == classes[classes.Count - 2]) { restricoesDaCampanha.text += " e "; }
            else if (classe == classes[classes.Count - 1]) { continue; }
            else { restricoesDaCampanha.text += ", "; }
        }
        restricoesDaCampanha.text += ".";
    }

    public void configurarRestricoesEObjetivos(){
        Campanha campanhaAtual = BancoDeDados.instancia.campanhasDoJogo[campanhaSelecionada];
        if (campanhaAtual.limitacoesDaCampanha.quantidadeDeUnidades == 0) { restricoesDaCampanha.text = "Não há limite no numero de brigadas. \n"; }
        else { restricoesDaCampanha.text = "Você pode usar no maximo " + campanhaAtual.limitacoesDaCampanha.quantidadeDeUnidades + " de brigadas distribuidas entre suas divisões"; }
        foreach(Vector2 restricao in campanhaAtual.limitacoesDaCampanha.classesRestringidas){
            Classe classe = (Classe)restricao[0];
            associarEnumParaNome(restricoesDaCampanha, classe, (int)restricao[1]);
        }
        listarClassesProibidas(campanhaAtual.limitacoesDaCampanha.classesProibidas);
        objetivos.text = "";
        for(int i = 0; i < campanhaAtual.victoryConditions.Count; i++){
            objetivos.text += "Conquistar o(s) ponto(s) estrategico(s) ";
            for(int j = 0; j < campanhaAtual.victoryConditions[i].Count(); j++){
                switch (campanhaSelecionada){
                    case 0:
                        objetivos.text += (Mundo0)campanhaAtual.victoryConditions[i][j];
                        break;
                    case 1:
                        objetivos.text += (Mundo1)campanhaAtual.victoryConditions[i][j];                        
                        break;
                }

                if (j + 1 >= campanhaAtual.victoryConditions[i].Count()) { objetivos.text += ".";  }
                else { objetivos.text += " e "; }
            }
            if(!(i+1 >= campanhaAtual.victoryConditions.Count)) { objetivos.text += " ou \n";  }
        }

        if (!verificarRestricoes()){
            feedbackDasRestricoes.text = "Rejeitado";
            feedbackDasRestricoes.color = new Color(255,0,0);
            startCampanha.interactable = false;
        }
        else{
            feedbackDasRestricoes.text = "Permitido";
            feedbackDasRestricoes.color = new Color(0, 100, 0);
            startCampanha.interactable = true;
        }
    }

    public bool verificarRestricoes(){
        //Primeiro restrição obvia, se o jogador tem ao menos 1 unidades no seu exercito
        if((GameController.instancia.primeiroBatalhao.Count + GameController.instancia.segundoBatalhao.Count + 
            GameController.instancia.terceiroBatalhao.Count) == 0){
            return false;
        }
        Campanha campanhaAtual = BancoDeDados.instancia.campanhasDoJogo[campanhaSelecionada];

        List<Unidade> todasAsUnidades = new List<Unidade>(GameController.instancia.primeiroBatalhao);
        todasAsUnidades.AddRange(GameController.instancia.segundoBatalhao);
        todasAsUnidades.AddRange(GameController.instancia.terceiroBatalhao);
        //Verificar se ele passou do limite de unidades
        if (campanhaAtual.limitacoesDaCampanha.quantidadeDeUnidades != 0 && 
            todasAsUnidades.Count > campanhaAtual.limitacoesDaCampanha.quantidadeDeUnidades){
            return false;
        }
        //Verificar se ele está usando alguma classe proibida
        foreach(Classe classe in campanhaAtual.limitacoesDaCampanha.classesProibidas){
            if(todasAsUnidades.Any(x => x.classe == classe)){
                return false;
            }
        }        
        //Finalmente, verificar se ele passou do limite de alguma das classes restringidas
        foreach (Vector2 restricao in campanhaAtual.limitacoesDaCampanha.classesRestringidas){
            Classe classe = (Classe)restricao[0];
            int quantosDaClasse = todasAsUnidades.Where(x => x.classe == classe).Count();
            if (quantosDaClasse > restricao[1]){
                return false;
            }
        }
        return true;
    }

    public void EsconderDetalhesDaCampanha() {
        if (detalhes.activeSelf) {
            detalhes.SetActive(false);
            mascaraDeOpcoes.SetActive(false);
        }
    }

    public void AvancarParaCampanha(){
        detalhes.SetActive(false);
        GameController.instancia.campanhaSelecionada = campanhaSelecionada;
        SceneManager.LoadScene(BancoDeDados.instancia.campanhasDoJogo[campanhaSelecionada].cenaDaCampanha);
    }

    public void mudarBatalhao(int qual){
        selecaoDoBatalhao.sprite = batalhaoSelecionadoSprites[qual - 1];
        batalhaoSelecionado = qual;
        PopularAmostragem();
    }

    public void PopularAmostragem(){
        switch (batalhaoSelecionado){
            case 1:
                for(int i=0;i<6;i++){
                    if(i<GameController.instancia.primeiroBatalhao.Count)amostragem[i].sprite = BancoDeDados.instancia.visualSecundarioUnidadesDoJogo[GameController.instancia.primeiroBatalhao[i].idNoBancoDeDados];
                    else amostragem[i].sprite= NoArmyAssigned;
                }
                break;
            case 2:
                for(int i=0;i<6;i++){
                    if(i<GameController.instancia.segundoBatalhao.Count)amostragem[i].sprite= BancoDeDados.instancia.visualSecundarioUnidadesDoJogo[GameController.instancia.segundoBatalhao[i].idNoBancoDeDados];
                    else amostragem[i].sprite= NoArmyAssigned;
                }
            break;
            case 3:
                for(int i=0;i<6;i++){
                    if(i<GameController.instancia.terceiroBatalhao.Count)amostragem[i].sprite= BancoDeDados.instancia.visualSecundarioUnidadesDoJogo[GameController.instancia.terceiroBatalhao[i].idNoBancoDeDados];
                    else amostragem[i].sprite= NoArmyAssigned;
                }
            break;
        }
    }

    public void mudarCampanhaSelecionada(int qual){
        campanhaSelecionada = qual;
        mudarOutline(qual);
        Pais paisDaCampanha = BancoDeDados.instancia.campanhasDoJogo[qual].paisDaCampanha;
        iconeDaNacaoDaCampanha.sprite = BancoDeDados.instancia.iconesDeNacoes[(int)paisDaCampanha];
        nomeDaNacaoDaCampanha.sprite = BancoDeDados.instancia.nomesDeNacoes[(int)paisDaCampanha];
        descricao.text = "\n" + BancoDeDados.instancia.campanhasDoJogo[qual].descricao;
        imagemDaCampanha.sprite = imagensDeDemonstracaoDaCampanha[qual];
    }

    public void mudarOutline(int qual){
        for(int i = 0; i < campanhasBotoesHolder.transform.childCount; i++){
            if(i == qual){
                campanhasBotoesHolder.transform.GetChild(i).GetComponent<Outline>().enabled = true;
            }
            else{
                campanhasBotoesHolder.transform.GetChild(i).GetComponent<Outline>().enabled = false;
            }
        }
    }

    public void ChangeScrollPos(){
        conteudoDeScroll.verticalNormalizedPosition = sliderDePosicaoDoConteudo.value;
    }

    void adicionarEventTriggerDeSFX(GameObject target){
        EventTrigger triggerDeSons = target.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry hover = new EventTrigger.Entry();
        hover.eventID = EventTriggerType.PointerEnter;
        hover.callback.AddListener((eventdata) => { EventosDoHQ.instancia.HoverDeBotaoMetalico(); });
        triggerDeSons.triggers.Add(hover);

        EventTrigger.Entry click = new EventTrigger.Entry();
        click.eventID = EventTriggerType.PointerClick;
        click.callback.AddListener((eventdata) => { EventosDoHQ.instancia.ClickEmBotaoDePapel(); });
        triggerDeSons.triggers.Add(click);
    }
    
    public void AtivarCampanhas(){
        for(int i=0;i<BancoDeDados.instancia.campanhasDoJogo.Count;i++){
            if (BancoDeDados.instancia.campanhasDoJogo[i].ativo){
                campanhas[i].interactable = true;
            }
            else{
                campanhas[i].interactable = false;
            }
        }
    }

    private void associarNomeParaClasse(Classe classe, Text ondeEscrever){
        switch (classe){
            case Classe.Infantaria: ondeEscrever.text += "Infantaria Classica"; break;
            case Classe.InfantariaNaval: ondeEscrever.text += "Fuzileiro Naval"; break;
            case Classe.InfantariaAlpina: ondeEscrever.text += "Infantaria Alpina"; break;
            case Classe.Artilharia: ondeEscrever.text += "Artilharia"; break;
            case Classe.AntiTank: ondeEscrever.text += "Antitanque"; break;
            case Classe.RocketArtilharia: ondeEscrever.text += "Artilharia de Foguetes"; break;
            case Classe.Cavalaria: ondeEscrever.text += "Infantaria Montada"; break;
            case Classe.FullMecanizada: ondeEscrever.text += "Infantaria Mecanizada"; break;
            case Classe.FullMotorizada: ondeEscrever.text += "Infantaria Motorizada"; break;
            case Classe.SemiMecanizada: ondeEscrever.text += "Infantaria Semi-Mecanizada"; break;
            case Classe.SemiMotorizada: ondeEscrever.text += "Infantaria Semi-Motorizada"; break;
            case Classe.TankDestroyer: ondeEscrever.text += "Destruidor de Tanques"; break;
            case Classe.SPArtilharia: ondeEscrever.text += "Artilharia Automotora"; break;
            case Classe.SPRocketArtilharia: ondeEscrever.text += "Artilharia de Foguetes Automotora"; break;
            case Classe.TanqueLeve: ondeEscrever.text += "Tanque Leve"; break;
            case Classe.TanqueMedio: ondeEscrever.text += "Tanque Medio"; break;
            case Classe.TanquePesado: ondeEscrever.text += "Tanque Pesado"; break;
            case Classe.TanqueUltraPesado: ondeEscrever.text += "Tanque Ultra Pesado"; break;
        }
    }

    private void associarEnumParaNome(Text ondeEscrever, Classe classe , int restricao){
        ondeEscrever.text += "Permitido o uso de " + restricao + " brigadas de ";
        associarNomeParaClasse(classe, ondeEscrever);
        ondeEscrever.text += ".\n";
    }
}
