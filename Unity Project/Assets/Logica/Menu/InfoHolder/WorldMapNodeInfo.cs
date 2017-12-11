using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Assets.Logica.Classes;

public enum Terrenos{
    Colinas,
    Deserto,
    Bosques,
    Gelado,
    Congelado,
    Pantâno,
    Terroso,
    Urbano,
    Montanha,
    Planice
}

public enum QualidadeDeSuprimento{
    Destruida=1,
    Contestada,
    Regularizada,
    Efetiva,
    Excelente
}

public class WorldMapNodeInfo : MonoBehaviour {
    private Node no;
    public Text nome;
    public Text status;
    public Text temperatura;
    public Text terreno;
    public Text pontoEstrategico;
    public Image terrenoImage;
    public Image imagemDemonstrativa;
    public Terrenos terrenoDoNode;
    public Sprite[] terrenos;
    public Sprite[] imagensDemonstrativas;

    public GameObject inteligenciaHolder;
    public GameObject opcoesDeAtaque;
    public Text numeroDeInimigos;
    public Text classeMajoritaria;
    public Text classeMinoritaria;
    public Text numeroDeSuporte;
    public Button Blitz;
    public Button Organizada;

    public GameObject suprimentosHolder;
    public GameObject opcoesDeSuprimento;
    public QualidadeDeSuprimento qualidadeDeSuprimento;
    public Text combustivelNescessario;
    public Text municaoNescessaria;
    public Text reservasNescessaria;
    public Text qualidadeDoSuprimento;
    public Text feedbackEfetividade;

    public Button recarregarSuprimentos;

    public int custoTotalMunicao;
    public int custoTotalCombustivel;
    public int custoTotalReservas;
    
    public void popularInfoBasica(Node no) {//E chamado toda vez que o node e entrado, popula as informações básicas
        this.no = no;
        switch (GameController.instancia.campanhaSelecionada) {
            case 0:
                nome.text = no.nodeNoMundo0.ToString();
                break;
            case 1:
                nome.text = no.nodeNoMundo1.ToString();
                break;
        }
        if(no.parteDoReich && no.conquistado) { status.text = "Terra mãe"; imagemDemonstrativa.sprite = imagensDemonstrativas[0]; }
        else if (no.conquistado && no.armazem && !no.recentementeConquistada) { status.text = "Armazém"; imagemDemonstrativa.sprite = imagensDemonstrativas[1]; }
        else if (no.conquistado && !no.recentementeConquistada) { status.text = "Ocupado"; imagemDemonstrativa.sprite = imagensDemonstrativas[1]; }
        else if (no.conquistado && no.recentementeConquistada) { status.text = "Resistência"; imagemDemonstrativa.sprite = imagensDemonstrativas[2]; }
        else if (!no.conquistado) { status.text = "Hostil"; imagemDemonstrativa.sprite = imagensDemonstrativas[3]; }
        temperatura.text = no.calcularTemperatura()+"°";
        terreno.text = no.terreno.ToString();
        terrenoImage.sprite = terrenos[(int)no.terreno];
        if (no.pontoEstrategico) { pontoEstrategico.text = "Sim"; }
        else { pontoEstrategico.text = "Não"; }       
    }

    public void popularInteligencia(){
        if (!inteligenciaHolder.activeSelf) { inteligenciaHolder.SetActive(true); }
        if (suprimentosHolder.activeSelf) { suprimentosHolder.SetActive(false); }
        List<Unidade> inimigosNoJogo = new List<Unidade>();
        int numeroDeInimigos=0;
        int numeroDeSuporte=0;

        MapXmlContainer container;
        container = MapSaveLoad.OnGameLoad(1);
        foreach (UnidadeXml uXML in container.unidadesNoTabuleiro){
            inimigosNoJogo.Add(BancoDeDados.instancia.retornarUnidade(uXML.id));
        }
        container = MapSaveLoad.OnGameLoad(2);
        foreach (UnidadeXml uXML in container.unidadesNoTabuleiro){
            numeroDeSuporte++;
            inimigosNoJogo.Add(BancoDeDados.instancia.retornarUnidade(uXML.id));
        }
        container = MapSaveLoad.OnGameLoad(3);
        foreach (UnidadeXml uXML in container.unidadesNoTabuleiro){            
            inimigosNoJogo.Add(BancoDeDados.instancia.retornarUnidade(uXML.id));
        }

        numeroDeInimigos = inimigosNoJogo.Count;
        

        classeMajoritaria.text = VerificarClasseMajoritariaOuMinoritaria(inimigosNoJogo,true).ToString();
        classeMinoritaria.text = VerificarClasseMajoritariaOuMinoritaria(inimigosNoJogo, false).ToString();
        this.numeroDeInimigos.text = numeroDeInimigos.ToString();
        this.numeroDeSuporte.text = numeroDeSuporte.ToString();
        if (!opcoesDeAtaque.activeSelf) { opcoesDeAtaque.SetActive(true); }
        if (opcoesDeSuprimento.activeSelf) { opcoesDeSuprimento.SetActive(false); }

        if (GameController.instancia.primeiroBatalhao.Count == 0)
            Blitz.interactable = false;
        else
            Blitz.interactable = true;

        if ((GameController.instancia.segundoBatalhao.Count + GameController.instancia.terceiroBatalhao.Count) == 0)
            Organizada.interactable = false;
        else
            Organizada.interactable = true;
    }

    public Classe VerificarClasseMajoritariaOuMinoritaria(List<Unidade> inimigosDaPartida, bool majoritarioOuMinoritario){
        List<Classe> classes = new List<Classe>();
        List<int> quantidades = new List<int>();
        foreach(Unidade u in inimigosDaPartida){
            if(classes.Contains(u.classe)){
                quantidades[classes.IndexOf(u.classe)] = quantidades[classes.IndexOf(u.classe)] + 1 ;
            }
            else{
                classes.Add(u.classe);
                quantidades.Add(1);
            }
        }
        if (majoritarioOuMinoritario) { return classes[quantidades.IndexOf(quantidades.Max())]; }
        else { return classes[quantidades.IndexOf(quantidades.Min())]; }
    }

    public void popularSuprimentos(){//Só e chamado se o node estiver conquistado, serve para permitir o suprimento das tropas
        if (inteligenciaHolder.activeSelf) { inteligenciaHolder.SetActive(false); }
        if (!suprimentosHolder.activeSelf) { suprimentosHolder.SetActive(true); }

        if (no.armazem && no.parteDoReich) { qualidadeDeSuprimento = QualidadeDeSuprimento.Excelente; }
        else if (no.parteDoReich) { qualidadeDeSuprimento = QualidadeDeSuprimento.Efetiva; }
        else if (no.armazem && no.conquistado) { qualidadeDeSuprimento = QualidadeDeSuprimento.Regularizada; }
        else if (no.conquistado) { qualidadeDeSuprimento = QualidadeDeSuprimento.Contestada; }
        else if (no.conquistado && no.recentementeConquistada) { qualidadeDeSuprimento = QualidadeDeSuprimento.Destruida; }
        qualidadeDoSuprimento.text = qualidadeDeSuprimento.ToString();

        switch (qualidadeDeSuprimento){
            case QualidadeDeSuprimento.Destruida:
                feedbackEfetividade.text = "(5% por Dia)";
                calcularCustos(5);
                break;
            case QualidadeDeSuprimento.Contestada:
                feedbackEfetividade.text = "(10% por Dia)";
                calcularCustos(10);
                break;
            case QualidadeDeSuprimento.Regularizada:
                feedbackEfetividade.text = "(15% por Dia)";
                calcularCustos(15);
                break;
            case QualidadeDeSuprimento.Efetiva:
                feedbackEfetividade.text = "(20% por Dia)";
                calcularCustos(20);
                break;
            case QualidadeDeSuprimento.Excelente:
                feedbackEfetividade.text = "(25% por Dia)";
                calcularCustos(25);
                break;
        }
        mostrarCustos();
        if (opcoesDeAtaque.activeSelf) { opcoesDeAtaque.SetActive(false); }
        if (!opcoesDeSuprimento.activeSelf) { opcoesDeSuprimento.SetActive(true); }
    }

    private void calcularCustos(int grauDeRecarga){
        foreach (Unidade u in GameController.instancia.primeiroBatalhao){
            if (u.status[13] < 100 || u.status[15] < 100 || u.status[1] != u.status[0]){//A unidade não tem tudo que precisa
                if (u.status[13] < (100 - grauDeRecarga)) { custoTotalCombustivel += (u.status[12] * grauDeRecarga) / 100; }
                else { custoTotalCombustivel += (u.status[12] * (100 - u.status[13])) / 100; }

                if (u.status[15] < (100 - grauDeRecarga)) { custoTotalMunicao += (u.status[14] * grauDeRecarga) / 100; }
                else { custoTotalMunicao += (u.status[14] * (100 - u.status[15])) / 100; }

                if ((u.status[0] * grauDeRecarga) / 100 > u.status[0] - u.status[1]) { custoTotalReservas += u.status[16] - (((u.status[1] * 100) / u.status[0]) * u.status[16]) / 100; }
                else { custoTotalReservas += u.status[16] - ((100 - grauDeRecarga) * u.status[16]) / 100; }

            }
        }
        foreach (Unidade u in GameController.instancia.segundoBatalhao){
            if (u.status[13] < 100 || u.status[15] < 100 || u.status[1] != u.status[0]){//A unidade não tem tudo que precisa

                if (u.status[13] < (100 - grauDeRecarga)) { custoTotalCombustivel += (u.status[12] * grauDeRecarga) / 100; }
                else { custoTotalCombustivel += (u.status[12] * (100 - u.status[13])) / 100; }

                if (u.status[15] < (100 - grauDeRecarga)) { custoTotalMunicao += (u.status[14] * grauDeRecarga) / 100; }
                else { custoTotalMunicao += (u.status[14] * (100 - u.status[15])) / 100; }

                if ((u.status[0] * grauDeRecarga) / 100 > u.status[0] - u.status[1]) { custoTotalReservas += u.status[16] - (((u.status[1] * 100) / u.status[0]) * u.status[16]) / 100; }
                else { custoTotalReservas += u.status[16] - ((100 - grauDeRecarga) * u.status[16]) / 100; }

            }
        }
        foreach (Unidade u in GameController.instancia.terceiroBatalhao){
            if (u.status[13] < 100 || u.status[15] < 100 || u.status[1] != u.status[0]){//A unidade não tem tudo que precisa

                if (u.status[13] < (100 - grauDeRecarga)) { custoTotalCombustivel += (u.status[12] * grauDeRecarga) / 100; }
                else { custoTotalCombustivel += (u.status[12] * (100 - u.status[13])) / 100; }

                if (u.status[15] < (100 - grauDeRecarga)) { custoTotalMunicao += (u.status[14] * grauDeRecarga) / 100; }
                else { custoTotalMunicao += (u.status[14] * (100 - u.status[15])) / 100; }

                if ((u.status[0] * grauDeRecarga) / 100 > u.status[0] - u.status[1]) { custoTotalReservas += u.status[16] - (((u.status[1] * 100) / u.status[0]) * u.status[16]) / 100; }
                else { custoTotalReservas += u.status[16] - ((100 - grauDeRecarga) * u.status[16]) / 100; }
            }
        }

        if(custoTotalCombustivel == 0 && custoTotalMunicao== 0 && custoTotalReservas == 0){
            recarregarSuprimentos.interactable = false;
        }
        else { recarregarSuprimentos.interactable = true; }
    }

    public void efetivarCusto(){
        SoundPlayer.instancia.Recarregando();

        GameController.instancia.Combustivel -= custoTotalCombustivel;
        GameController.instancia.Municao -= custoTotalMunicao;
        GameController.instancia.MaoDeObra -= custoTotalReservas;

        int grauDeRecarga = (int)qualidadeDeSuprimento * 5;

        foreach (Unidade u in GameController.instancia.primeiroBatalhao){
            if (u.status[13] < (100 - grauDeRecarga)) { u.status[13] = 100; }
            else { u.status[13] += grauDeRecarga; }

            if (u.status[15] < (100 - grauDeRecarga)) { u.status[15] = 100; }
            else { u.status[15] += grauDeRecarga; }

            if ((u.status[0] * grauDeRecarga) / 100 > u.status[0] - u.status[1]) { u.status[1] = u.status[0]; }
            else { u.status[1] += (u.status[0] * grauDeRecarga) / 100; }
        }
        foreach (Unidade u in GameController.instancia.segundoBatalhao){
            if (u.status[13] < (100 - grauDeRecarga)) { u.status[13] = 100; }
            else { u.status[13] += grauDeRecarga; }

            if (u.status[15] < (100 - grauDeRecarga)) { u.status[15] = 100; }
            else { u.status[15] += grauDeRecarga; }

            if ((u.status[0] * grauDeRecarga) / 100 > u.status[0] - u.status[1]) { u.status[1] = u.status[0]; }
            else { u.status[1] += (u.status[0] * grauDeRecarga) / 100; }
        }
        foreach (Unidade u in GameController.instancia.terceiroBatalhao){
            if (u.status[13] < (100 - grauDeRecarga)) { u.status[13] = 100; }
            else { u.status[13] += grauDeRecarga; }

            if (u.status[15] < (100 - grauDeRecarga)) { u.status[15] = 100; }
            else { u.status[15] += grauDeRecarga; }

            if ((u.status[0] * grauDeRecarga) / 100 > u.status[0] - u.status[1]) { u.status[1] = u.status[0]; }
            else { u.status[1] += (u.status[0] * grauDeRecarga) / 100; }
        }
        custoTotalCombustivel = 0;
        custoTotalMunicao = 0;
        custoTotalReservas = 0;

        NavegacaoController.instancia.newDay();
        NavegacaoController.instancia.atualiazarHeader();
        popularSuprimentos();
    }

    private void mostrarCustos(){

        if (custoTotalReservas == 0) { reservasNescessaria.color = Color.green;}
        else { reservasNescessaria.color = Color.red; }
        if (custoTotalMunicao == 0) { municaoNescessaria.color = Color.green; }
        else { municaoNescessaria.color = Color.red; }
        if (custoTotalCombustivel == 0) { combustivelNescessario.color = Color.green; }
        else { combustivelNescessario.color = Color.red; }

        combustivelNescessario.text = custoTotalCombustivel.ToString();
        municaoNescessaria.text = custoTotalMunicao.ToString();
        reservasNescessaria.text = custoTotalReservas.ToString();
    } 
}
