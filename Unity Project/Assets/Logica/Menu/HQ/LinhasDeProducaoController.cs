using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Logica.Classes;
using System.Linq;
using UnityEngine.EventSystems;

public class LinhasDeProducaoController : MonoBehaviour {
    public static LinhasDeProducaoController instancia;
    public List<LinhaDeProducaoCarta> linhasAtivas;

    public Text linhasAtivasFeedback;
    public Text linhasProduzindo;
    public Text linhasIdle;

    public Image unidadeCompletadaCard;
    public Text[] statusUnidadeTerminada;
    public Text nomeDaUnidade;
    public Sprite unidadeVoid;

    public Slider sliderDePosicaoDeConteudo;
    public ScrollRect conteudoScroll;

    public Image simboloDaNacaoImage;
    public Image nomeDaNacao;
    public Image anoDoDesenvolvimento;
    public Text descricaoDoNivelTecnologico;

    public Image imagemDoEquipamento;
    public Text descricaoDoEquipamento;
    public Text nomeDoEquipamento;
    public Text[] statusDoEquipamentoCriado;
    public Sprite equipamentoVoid;

    public SpriteState coletarState;
    public SpriteState produzirState;
    public SpriteState trocarState;

    public Sprite coletarSprite;
    public Sprite produzirSprite;
    public Sprite trocarSprite;

    public GameObject linhasHolder;
    public GameObject promptHolder;

    public GameObject opcoesDeProducaoHolder;

    public GameObject recrutamentoHolder;
    public GameObject desenvolvimentoHolder;
    public GameObject construcaoHolder;
    public GameObject resultadoConstrucaoHolder;

    public GameObject resultadoRecrutamentoHolder;
    public GameObject resultadoDesenvolvimentoHolder;

    public GameObject mascaraDeOpcoes;
    public GameObject mascaraDeOpcoesAvancadas;

    public GameObject linhaDeProducaoCartaPrefab;
    public Sprite[] anosDeDesenvolvimento = new Sprite[7];

    public LinhaDeProducao linhaSelecionada;
    public bool escolhendoProducao = false;//Se o player está escolhendo algum recrutamento ou desenvolvimento

    void Awake(){
        instancia=this;
    }

    void Start(){
        foreach(LinhaDeProducao linha in GameController.instancia.linhasDeProducao){
            LinhaDeProducaoCarta novaCarta = Instantiate(linhaDeProducaoCartaPrefab).GetComponent<LinhaDeProducaoCarta>();
            linha.slot = GameController.instancia.linhasDeProducao.IndexOf(linha);
            adicionarEventTriggerDeSFX(novaCarta.trocarProducao.gameObject);
            novaCarta.linhaAssociada = linha;
            novaCarta.transform.SetParent(linhasHolder.transform,false);
            popularCarta(novaCarta);
            linhasAtivas.Add(novaCarta);
        }
        atualizarInterfaceDoFeedback ();
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

    public void atualizarVisual(LinhaDeProducao linhaAAtualizar){
        LinhaDeProducaoCarta linhaAModificar =  linhasAtivas.First(x => x.linhaAssociada.Equals(linhaAAtualizar));
        popularCarta(linhaAModificar);
        atualizarInterfaceDoFeedback();
    }

    public void mascaraClicada(){
        if (promptHolder.activeSelf) { promptHolder.SetActive(false); }
        if (opcoesDeProducaoHolder.activeSelf) { opcoesDeProducaoHolder.SetActive(false);GameController.instancia.despausarProducao(linhaSelecionada); }
        if (resultadoDesenvolvimentoHolder.activeSelf) { resultadoDesenvolvimentoHolder.SetActive(false); }
        if (resultadoRecrutamentoHolder.activeSelf) { resultadoRecrutamentoHolder.SetActive(false); }
        mascaraDeOpcoes.SetActive(false);
    }

    public void segundaMascaraClicada(){
        if (recrutamentoHolder.activeSelf) { recrutamentoHolder.SetActive(false); }
        if (desenvolvimentoHolder.activeSelf) { desenvolvimentoHolder.SetActive(false); }
        if (construcaoHolder.activeSelf) { construcaoHolder.SetActive(false); }
        mascaraDeOpcoesAvancadas.SetActive(false);
        efetivarTrocaDeProducao(-1);//Desativar linha de produção ja que o usuario desistiu da mudança mas ja confirmou que não liga pra essa linha
    }

    public void atualizarInterfaceDoFeedback(){
        int FeedbackLinhasAtivas = 0; int linhasEmProducao = 0; int linhasFazendoNada = 0;
        foreach (LinhaDeProducao linha in GameController.instancia.linhasDeProducao){
            FeedbackLinhasAtivas++;
            if (linha.ativa) { linhasEmProducao++; }
            else { linhasFazendoNada++; }
        }

        linhasAtivasFeedback.text = FeedbackLinhasAtivas.ToString();
        linhasProduzindo.text = linhasEmProducao.ToString();
        linhasIdle.text = linhasFazendoNada.ToString();
    }


    public void popularCarta(LinhaDeProducaoCarta cartaDaLinha){
        if (cartaDaLinha.linhaAssociada.terminada){
            cartaDaLinha.imagemBaseBotao.sprite = coletarSprite;
            cartaDaLinha.trocarProducao.spriteState = coletarState;
            cartaDaLinha.engrenagens[0].SetBool("emAtividade", false);
            cartaDaLinha.engrenagens[1].SetBool("emAtividade", false);
            cartaDaLinha.ocupadotexto.text = "Terminada";
            switch (cartaDaLinha.linhaAssociada.tipoDeProducao){
                case ProducaoTipo.Recrutamento:
                    cartaDaLinha.proguessoNumero.text = "100%";
                    cartaDaLinha.barraDeProguesso.value = 100;
                    cartaDaLinha.imagemProducao.sprite = Resources.Load<Sprite>("Graficos/Texturas/Imagens/HQ/LinhasDeProducao/Recrutamento");
                    cartaDaLinha.descricaoProducao.text = "Recrutamento";
                    break;
                case ProducaoTipo.Desenvolvimento:
                    cartaDaLinha.proguessoNumero.text = "100%";
                    cartaDaLinha.barraDeProguesso.value = 100;
                    cartaDaLinha.imagemProducao.sprite = Resources.Load<Sprite>("Graficos/Texturas/Imagens/HQ/LinhasDeProducao/Development");
                    cartaDaLinha.descricaoProducao.text = "Desenvolvimento";
                    break;
                case ProducaoTipo.Construcao:
                    cartaDaLinha.proguessoNumero.text = "100%";
                    cartaDaLinha.barraDeProguesso.value = 100;
                    cartaDaLinha.imagemProducao.sprite = Resources.Load<Sprite>("Graficos/Texturas/Imagens/HQ/LinhasDeProducao/EquipmentProduction");
                    cartaDaLinha.descricaoProducao.text = "Construção";
                    break;
            }
        }
        else if (cartaDaLinha.linhaAssociada.ativa){
            cartaDaLinha.imagemBaseBotao.sprite = trocarSprite;
            cartaDaLinha.trocarProducao.spriteState = trocarState;
            cartaDaLinha.engrenagens[0].SetBool("emAtividade", true);
            cartaDaLinha.engrenagens[1].SetBool("emAtividade", true);
            cartaDaLinha.ocupadotexto.text = "Ocupada";           
            switch(cartaDaLinha.linhaAssociada.tipoDeProducao){
                case ProducaoTipo.Metais:
                    cartaDaLinha.barraDeProguesso.value = 100;
                    cartaDaLinha.proguessoNumero.text = "∞";
                    cartaDaLinha.imagemProducao.sprite = Resources.Load<Sprite>("Graficos/Texturas/Imagens/HQ/LinhasDeProducao/MetalWorks");
                    cartaDaLinha.descricaoProducao.text = "Metais";
                    break;
                case ProducaoTipo.Combustivel:
                    cartaDaLinha.barraDeProguesso.value = 100;
                    cartaDaLinha.proguessoNumero.text = "∞";
                    cartaDaLinha.imagemProducao.sprite = Resources.Load<Sprite>("Graficos/Texturas/Imagens/HQ/LinhasDeProducao/OilField");
                    cartaDaLinha.descricaoProducao.text = "Combustivel";
                    break;
                case ProducaoTipo.Municao:
                    cartaDaLinha.barraDeProguesso.value = 100;
                    cartaDaLinha.proguessoNumero.text = "∞";
                    cartaDaLinha.imagemProducao.sprite = Resources.Load<Sprite>("Graficos/Texturas/Imagens/HQ/LinhasDeProducao/AmmoProduction");
                    cartaDaLinha.descricaoProducao.text = "Munição";
                    break;
                case ProducaoTipo.BensDeConsumo:
                    cartaDaLinha.barraDeProguesso.value = 100;
                    cartaDaLinha.proguessoNumero.text = "∞";
                    cartaDaLinha.imagemProducao.sprite = Resources.Load<Sprite>("Graficos/Texturas/Imagens/HQ/LinhasDeProducao/ConsumerGoods");
                    cartaDaLinha.descricaoProducao.text = "Bens de Consumo";
                    break;
                case ProducaoTipo.Recrutamento:
                    cartaDaLinha.barraDeProguesso.value = (int)cartaDaLinha.linhaAssociada.proguesso;
                    cartaDaLinha.proguessoNumero.text = (int)cartaDaLinha.linhaAssociada.proguesso + "%";
                    cartaDaLinha.imagemProducao.sprite = Resources.Load<Sprite>("Graficos/Texturas/Imagens/HQ/LinhasDeProducao/Recrutamento");
                    cartaDaLinha.descricaoProducao.text = "Recrutamento";
                    break;
                case ProducaoTipo.Desenvolvimento:
                    cartaDaLinha.barraDeProguesso.value = (int)cartaDaLinha.linhaAssociada.proguesso;
                    cartaDaLinha.proguessoNumero.text = (int)cartaDaLinha.linhaAssociada.proguesso + "%";
                    cartaDaLinha.imagemProducao.sprite = Resources.Load<Sprite>("Graficos/Texturas/Imagens/HQ/LinhasDeProducao/Development");
                    cartaDaLinha.descricaoProducao.text = "Desenvolvimento";
                    break;
                case ProducaoTipo.Construcao:
                    cartaDaLinha.barraDeProguesso.value = (int)cartaDaLinha.linhaAssociada.proguesso;
                    cartaDaLinha.proguessoNumero.text = (int)cartaDaLinha.linhaAssociada.proguesso + "%";
                    cartaDaLinha.imagemProducao.sprite = Resources.Load<Sprite>("Graficos/Texturas/Imagens/HQ/LinhasDeProducao/EquipmentProduction");
                    cartaDaLinha.descricaoProducao.text = "Construção";
                    break;
            }
        }
        else{
            cartaDaLinha.imagemBaseBotao.sprite = produzirSprite;
            cartaDaLinha.trocarProducao.spriteState = produzirState;
            cartaDaLinha.engrenagens[0].SetBool("emAtividade", false);
            cartaDaLinha.engrenagens[1].SetBool("emAtividade", false);
            cartaDaLinha.ocupadotexto.text = "Livre";
            cartaDaLinha.proguessoNumero.text = "∞";
            cartaDaLinha.imagemProducao.sprite = Resources.Load<Sprite>("Graficos/Texturas/Imagens/HQ/LinhasDeProducao/IdleFactory");
            cartaDaLinha.descricaoProducao.text = "Inativa";
            cartaDaLinha.barraDeProguesso.value=0;
        }
        cartaDaLinha.trocarProducao.GetComponent<Button>().onClick.RemoveAllListeners();
        cartaDaLinha.trocarProducao.GetComponent<Button>().onClick.AddListener(() => trocarProducao(cartaDaLinha.linhaAssociada.slot));
        cartaDaLinha.trocarProducao.GetComponent<Button>().onClick.AddListener(() => recuperarProducaoTerminada(cartaDaLinha.linhaAssociada.terminada));
    }

    public void trocarProducao(int slotClicado){
        linhaSelecionada = linhasAtivas[slotClicado].linhaAssociada;
        if (linhaSelecionada.terminada) {
            return;
        }
        else if (linhasAtivas[slotClicado].linhaAssociada.ativa){
            apresentarPrompt();
        }
        else{
            mostrarOpcoesDeProducao();
        }
        mascaraDeOpcoes.SetActive(true);
    }

    public void despausar(){
        if (linhaSelecionada.tipoDeProducao != ProducaoTipo.Inativa)
            GameController.instancia.despausarProducao(linhaSelecionada);
        else
            linhaSelecionada.ativa = false;
    }

    public void recuperarProducaoTerminada(bool terminada){
        if (terminada){
            //So pode ter no maximo 30 unidades na reserva, portanto ignorar o pedido de recuperação de produção
            if(linhaSelecionada.tipoDeProducao == ProducaoTipo.Recrutamento && GameController.instancia.reservas.Count < 30){
                if (linhaSelecionada.unidadeSendoProduzida != null)
                    GameController.instancia.AdicionarUnidadeAoJogador(linhaSelecionada.unidadeSendoProduzida);

                popularTelaDeResultadoDoRecrutamento(linhaSelecionada.unidadeSendoProduzida);                
                linhaSelecionada.unidadeSendoProduzida = null;
                mostrarResultadoRecrutamento();
                SoundHolder.instancia.MarchaDeSoldados();
            }
            else if(linhaSelecionada.tipoDeProducao == ProducaoTipo.Desenvolvimento){
                linhaSelecionada.nacaoSendoDesenvolvida.emDesenvolvimento = false;

                int index = GameController.instancia.nacoesDoJogador.FindIndex(country => country.pais == linhaSelecionada.nacaoSendoDesenvolvida.pais);
                GameController.instancia.nacoesDoJogador[index].nivel++;
                
                popularTelaDeResultadoDoDesenvolvimento(linhaSelecionada.nacaoSendoDesenvolvida);
                linhaSelecionada.nacaoSendoDesenvolvida = null;
                mostrarResultadoDesenvolvimento();
                SoundHolder.instancia.AguaFervendo();
            }
            //Mesmo esquema, no maximo 150 equipamentos no armazem, contudo esse numero não inclue aqueles atualmente equipados
            else if (linhaSelecionada.tipoDeProducao == ProducaoTipo.Construcao && GameController.instancia.reservas.Count < 150){
                if (linhaSelecionada.equipamentoSendoConstruido != null)
                    GameController.instancia.equipamentos.Add(linhaSelecionada.equipamentoSendoConstruido);

                popularTelaDeResultadoDaConstrucao(linhaSelecionada.equipamentoSendoConstruido);
                linhaSelecionada.equipamentoSendoConstruido = null;
                mostrarResultadoConstrucao();
                SoundHolder.instancia.Construcao();
            }
            else{ return; }
            linhaSelecionada.terminada = false;
            linhaSelecionada.ativa = false;
            linhaSelecionada.emEspera = false;
            linhaSelecionada.proguesso = 0;
            linhaSelecionada.ppm = 0;
            linhaSelecionada.tipoDeProducao = ProducaoTipo.Inativa;
            atualizarVisual(linhaSelecionada);
        }
    }

    public void popularTelaDeResultadoDoRecrutamento(Unidade unidadeRecrutada){
        if (unidadeRecrutada == null){
            nomeDaUnidade.text = "Falha no Recrutamento";
            imagemDoEquipamento.sprite = unidadeVoid;

            for (int i = 0; i < 11; i++){
                statusUnidadeTerminada[i].text = "-";
            }
            return;
        }

        nomeDaUnidade.text = unidadeRecrutada.nome;
        unidadeCompletadaCard.sprite = BancoDeDados.instancia.visualPrincipalUnidadesDoJogo[unidadeRecrutada.idNoBancoDeDados];
        for (int i = 0; i < 11; i++){
            if(i==0) statusUnidadeTerminada[i].text = unidadeRecrutada.status[i].ToString();
            else statusUnidadeTerminada[i].text = unidadeRecrutada.status[i+1].ToString();
        }
    }

    public void popularTelaDeResultadoDoDesenvolvimento(Nacoes nacaoDesenvolvida){
        nomeDaNacao.sprite = BancoDeDados.instancia.nomesDeNacoes[(int)nacaoDesenvolvida.pais];
        simboloDaNacaoImage.sprite = BancoDeDados.instancia.iconesDeNacoes[(int)nacaoDesenvolvida.pais];
        anoDoDesenvolvimento.sprite = anosDeDesenvolvimento[nacaoDesenvolvida.nivel];
        descricaoDoNivelTecnologico.text = nacaoDesenvolvida.descricoesTecnologicas[nacaoDesenvolvida.nivel];
    }

    public void popularTelaDeResultadoDaConstrucao(Equipamento equipamentoCriado){
        if (equipamentoCriado == null){
            nomeDoEquipamento.text = "Falha na Criação";
            descricaoDoEquipamento.text = "Devido a algum problema durante a construção dos equipamentos eles não poderam ser utilizados, a construção falhou.";
            imagemDoEquipamento.sprite = equipamentoVoid;

            for (int i = 0; i < 11; i++){
                statusDoEquipamentoCriado[i].text = "-";
                statusDoEquipamentoCriado[i].color = Color.black;
            }
            return;
        }

        nomeDoEquipamento.text = equipamentoCriado.nome;
        descricaoDoEquipamento.text = equipamentoCriado.descricao;
        imagemDoEquipamento.sprite = BancoDeDados.instancia.equipamentosDoJogoVisual[equipamentoCriado.id];
        //GAMBIARRA START
        int contadorAuxiliar = 0;
        bool extraAdicionado = false;

        for (int i = 0; i < 11; i++){            
            if (!extraAdicionado && i != 0){
                contadorAuxiliar++;
                extraAdicionado = true;
            }
            //GAMBIARRA END
            if (equipamentoCriado.statusDoEquipamento[contadorAuxiliar] < 0 && i==0){
                statusDoEquipamentoCriado[i].text = "-";
                statusDoEquipamentoCriado[i].color = Color.red;
            }
            else if (equipamentoCriado.statusDoEquipamento[contadorAuxiliar] == 0){
                statusDoEquipamentoCriado[i].text = "";
                statusDoEquipamentoCriado[i].color = Color.black;
            }
            else{
                statusDoEquipamentoCriado[i].text = "+";
                statusDoEquipamentoCriado[i].color = Color.green;
            }

            statusDoEquipamentoCriado[i].text += equipamentoCriado.statusDoEquipamento[contadorAuxiliar].ToString();
            contadorAuxiliar++;
        }
    }


    public void efetivarTrocaDeProducao(int tipoDeProducao){
        switch (tipoDeProducao){
            case (int)ProducaoTipo.Metais: linhaSelecionada.tipoDeProducao = ProducaoTipo.Metais; GameController.instancia.iniciarProducaoDeRecursos(linhaSelecionada); break;
            case (int)ProducaoTipo.Combustivel: linhaSelecionada.tipoDeProducao = ProducaoTipo.Combustivel; GameController.instancia.iniciarProducaoDeRecursos(linhaSelecionada); break;
            case (int)ProducaoTipo.Municao: linhaSelecionada.tipoDeProducao = ProducaoTipo.Municao; GameController.instancia.iniciarProducaoDeRecursos(linhaSelecionada); break;
            case (int)ProducaoTipo.BensDeConsumo: linhaSelecionada.tipoDeProducao = ProducaoTipo.BensDeConsumo; GameController.instancia.iniciarProducaoDeRecursos(linhaSelecionada); break;
            case (int)ProducaoTipo.Recrutamento: linhaSelecionada.tipoDeProducao = ProducaoTipo.Recrutamento; linhaSelecionada.emEspera = true; break;
            case (int)ProducaoTipo.Desenvolvimento: linhaSelecionada.tipoDeProducao = ProducaoTipo.Desenvolvimento; linhaSelecionada.emEspera = true;  break;
            case (int)ProducaoTipo.Construcao: linhaSelecionada.tipoDeProducao = ProducaoTipo.Construcao; linhaSelecionada.emEspera = true; break;
            case -1: linhaSelecionada.tipoDeProducao = ProducaoTipo.Inativa; linhaSelecionada.ativa = false; break;           
        }
        atualizarInterfaceDoFeedback();
        mascaraDeOpcoes.SetActive(false);
        LinhaDeProducaoCarta carta = linhasAtivas.First(x => x.linhaAssociada == linhaSelecionada);
        popularCarta(carta);
    }

    //Alterações de Display

    public void ChangeScrollPos(){
        conteudoScroll.verticalNormalizedPosition = sliderDePosicaoDeConteudo.value;
    }

    public void apresentarPrompt(){
        promptHolder.SetActive(true);
    }

    public void promptClicado(bool aceito){
        promptHolder.SetActive(false);
        if (aceito) { linhaSelecionada.emEspera = true; mostrarOpcoesDeProducao(); }
        else {
            mascaraDeOpcoes.SetActive(false);
            if(linhaSelecionada.tipoDeProducao == ProducaoTipo.Inativa){
                linhaSelecionada.ativa = false;
                linhaSelecionada.emEspera = false;
            }
        }
    }

    public void mostrarResultadoRecrutamento(){
        resultadoRecrutamentoHolder.SetActive(true);
    }

    public void esconderResultadoRecrutamento(){
        resultadoRecrutamentoHolder.SetActive(false);
    }

    public void mostrarResultadoConstrucao(){
        resultadoConstrucaoHolder.SetActive(true);
    }

    public void esconderResultadoConstrucao(){
        resultadoConstrucaoHolder.SetActive(false);
    }

    public void mostrarResultadoDesenvolvimento(){
        resultadoDesenvolvimentoHolder.SetActive(true);
    }

    public void esconderResultadoDesenvolvimento(){
        resultadoDesenvolvimentoHolder.SetActive(false);
    }

    public void mostrarOpcoesDeProducao(){
        opcoesDeProducaoHolder.SetActive(true);
    }
    public void esconderOpcoesDeProducao(){
        opcoesDeProducaoHolder.SetActive(false);
    }

    public void MostrarRecrutamento(){
        mascaraDeOpcoesAvancadas.SetActive(true);
        recrutamentoHolder.SetActive(true);
        recrutamentoHolder.GetComponent<RecrutamentoController>().ApresentarVisualRecrutamento();
    }
    public void EsconderRecrutamento(){
        mascaraDeOpcoesAvancadas.SetActive(false);
        recrutamentoHolder.SetActive(false);
    }
    public void MostrarDesenvolvimento(){
        mascaraDeOpcoesAvancadas.SetActive(true);
        desenvolvimentoHolder.SetActive(true);
        desenvolvimentoHolder.GetComponent<DesenvolvimentoController>().ApresentarVisualDesenvolvimento();
    }
    public void EsconderDesenvolvimento(){
        mascaraDeOpcoesAvancadas.SetActive(false);
        desenvolvimentoHolder.SetActive(false);
    }

    public void MostrarConstrucao(){
        mascaraDeOpcoesAvancadas.SetActive(true);
        construcaoHolder.SetActive(true);
        construcaoHolder.GetComponent<ConstrucaoController>().ApresentarVisualConstrucao();
    }
    public void EsconderConstrucao(){
        mascaraDeOpcoesAvancadas.SetActive(false);
        construcaoHolder.SetActive(false);
    }
    //Estado em que so o objeto principal está aberto no Canvas
    public void EstadoOriginal(){
        EsconderConstrucao();
        EsconderDesenvolvimento();
        EsconderRecrutamento();
        esconderResultadoConstrucao();
        esconderResultadoDesenvolvimento();
        esconderResultadoRecrutamento();

        mascaraDeOpcoes.SetActive(false);
        mascaraDeOpcoesAvancadas.SetActive(false);
        promptHolder.SetActive(false);

        esconderOpcoesDeProducao();
    }

}
