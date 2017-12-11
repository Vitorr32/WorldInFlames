using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Logica.Classes;
using UnityEngine.SceneManagement;

/*A batalha tem até 3 fases, a primeira fase vai ser entre as duas spearheads, em conflito
    direto, caso um ganhe ele terá a oportunidade de avançar pra cima da unidade de suporte
    inimiga, impedindo que ela possa dar um bom suporte para a batalha do front

    Front e uma batalha entre os dois fronts, contando(ou não) com o suporte, essa é a batalha
    decesiva quando se trata da ocupação de um territorio, caso vença o node sera contado como
    vencido, caso perca o usuario recuara pro node seguro mais proximo
    
    Resumindo:
    Primeira Fase: Spearhead X Spearhead
    Segunda Fase*: Spearhead X Suporte 
    Terceira Fase: Front + Suporte X Front + Suporte

    Todas as fases tem um timer para se dizer, cada turno significa que 20 minutos passaram,
    uma batalha longa pode demorar varias horas ou até dias.

    As primeiras fases também tem tempos especificos, a fase Spearhead acontece de manhã cedo 0800,
    quando o general manda eles a frente para vencer em mobilidade o inimigo, ela dura até o front
    pecorrer 10 quilometros, dependendo da velocidade da unidade mais lenta do front

    A Segunda fase tambem continua desse timer, o player tem que dar o maximo de dano antes do seu
    front chegar

    Finalmente, durante a fase front em que o exercito ataca com força total, os turnos são "ilimitados"
    mas ao anoitecer o jogador tem a possibilidade de parar o ataque e esperar até o proximo dia(não podera
    ter outra fase 1 ou 2 se fizer), pois atacar de noite e um tremendo esforço, diminuindo precisão, movimentação,
    dano das unidades

    Se voltar, o jogador volta pra tela de Batalha na cena do Mundo, podendo decidir continuar a batalha ou voltar pro
    Node liberado mais proximo
    
    *Apenas se a primeira fase tiver um vencendor 
    */

class BatalhaController : MonoBehaviour{
    public static BatalhaController instancia;
    //public InterfaceController controladorDaInterface;
    /*
        A IA vai influenciar principalmente na fase 1
        Uma IA passiva vai se focar em defender seu suporte e de vez em quando ajudar o front
        Uma IA equilibrada vai focar tanto em defender o suporte quanto ajudar o front
        Uma IA agressiva vai esquecer seu suporte e focar em atacar o seu e de vez em quando ajudar o front
        */
    //Para gerir a Inteligencia Artificial
    //public int tipoDeIA;//Qual tipo de Behaivour essa IA expressa, 1 para passiva, 2 para equilibrada e 3 para agressiva
    public List<UnidadeNoTabuleiro> alvosPrioritarios = new List<UnidadeNoTabuleiro>();
    public List<Tile> pontosDeDefesa = new List<Tile>();//Pontos que a IA quer tentar defender da melhor forma
    public List<Tile> defesaDoSuporte = new List<Tile>();//Pontos que a IA quer defender para proteger seu suporte
    public List<Tile> posicoesDeReforco = new List<Tile>();//Pontos que a IA quer defender caso não queira que o inimigo refoce o front
    /*Qual coluna a Inteligencia Artificial vai focar em defender, caso o inimigo avance além dela
    Essa variavel vai diminuir de acordo para redefinir a prioridade defensiva*/
    public int linhaDeDefesa = 0;

    public int linhasTabuleiro;
    public int colunasTabuleiro;
    public GameObject tileDoTabuleiroPrefab;
    public GameObject UnidadePrefab;
    public GameObject textoFlutuantePrefab;

    //Para manter o atual tabuleiro e unidades arquivados
    public GameObject tabuleiroHolder;
    public GameObject unidadeHolder;

    public Material TileGridNormal;
    public Material TileGridMovimento;
    public Material TileGridAtaque;
    //Para administrar a saida da Batalha, true pra voltar para campanha, false para voltar pro Title Screen
    public bool saidaDaBatalha;

    //Para calcular a vitoria e qual a "nota" do jogador
    public int battleGrade = 0;
    public bool vitoria;
    public int danoTotalInimigo;
    public int danoTotalAliado;
    public int numeroDeInimigos;
    public int numeroDeInimigosDestruidos;
    public int numeroDeAliados;
    public int numeroDeAliadosDestruidos;
    //Para gerir os inimigos
    public List<Unidade> spearheadInimigo = new List<Unidade>();
    public List<Unidade> frontInimigo = new List<Unidade>();
    public List<Unidade> suporteInimigo = new List<Unidade>();
    //Para gerir o spawn das unidades da batalha
    public List<Tile> spawnReforco = new List<Tile>();//Onde as unidades que vão reforçar o front vão spawnar
    public List<Tile> spawnReforcoInimigo = new List<Tile>();//mesmo que acima, só que pro inimigo
    public List<Tile> spawnAliado = new List<Tile>();//Onde as unidades do aliado podem ser spawnadas
    public List<Tile> spawnSuporteInimigo = new List<Tile>();//Spawn do suporte inimigo
    public List<Tile> locaisDeRetiradaInimiga = new List<Tile>();//Tiles que os inimigos vão tentar fugir caso desesperados
    public List<Unidade> unidadesASpawnar = new List<Unidade>();
    public List<Unidade> unidadesDoReforco = new List<Unidade>();
    //Para as animações dos aliados e inimigos
    public RuntimeAnimatorController[] controladoresAliados;
    public RuntimeAnimatorController[] controladoresInimigos;
    public GameObject FeedbackUnidadeAtual;
    //Para o flow das fases da batalha
    public int faseDeBatalha;
    public int unidadeAtualIndex;
    public int pontosDeAcao = 2;
    public List<List<Tile>> tabuleiro = new List<List<Tile>>();
    public List<UnidadeNoTabuleiro> unidadesNoTabuleiro = new List<UnidadeNoTabuleiro>(); //Serve para os turnos
    //Variaveis de controle de turno
    public List<UnidadeNoTabuleiro> aliadosEmCampo = new List<UnidadeNoTabuleiro>();
    public List<UnidadeNoTabuleiro> inimigosEmCampo = new List<UnidadeNoTabuleiro>();
    //Variaveis de controle de fase
    public List<Unidade> antiSuporteBlitz;//Unidades que vão atacar o suporte inimigo
    //Controla a quantidade de turnos
    public int chegadaFront;//O horario no jogo que o front vai chegar a batalha, como preferir  
    public int horarioAtual;//Turno atual, serve para dar feedback ao usuario;

    public bool rushado = false;

    //Para iluminação da fase
    public bool noite = false;
    public GameObject luzDoSol;
    public GameObject rotatorDoSol;
    private float RotateSpeed = 5f;
    private float Radius = 10f;
    public Vector3 centroDoTabuleiro;
    private float anguloDoSol;
    //Para definir skyboxes
    public Skybox componenteSkybox;
    public Material[] skyboxesPossiveis;

    public int turno = 0;
    //Nescessarios para a logica do pre-battle
    public bool preBattle = true;
    //Unidade aliada que está sendo setada no prebattle
    public UnidadeNoTabuleiro unidadeSendoSetada;
    //Bonus e efeitos da batalha
    private bool blitzkriegBonus;

    void Awake(){
        instancia = this;
    }

    void Start(){
        preBattle = true;
        carregarTodasAsUnidadesInimigas();
        if (!GameController.instancia.rushFront){
            //A menos velocidade vai determinar quantos turnos leva pro front chegar a batalha caso ele vá pra fase de spearhead

            int menorVelocidade;
            if (GameController.instancia.segundoBatalhao.Count != 0)
                menorVelocidade = GameController.instancia.segundoBatalhao.Min(uni => uni.status[3]);
            else
                menorVelocidade = frontInimigo.Min(uni => uni.status[3]);


            int horasNescessarias = 25 / menorVelocidade;
            if(horasNescessarias == 0) { horasNescessarias = 1; }
        
            rushado = false;
            faseDeBatalha = 1;
            horarioAtual = 0800;
            chegadaFront = horasNescessarias * 100 + horarioAtual;//1 turno e 1 hora, cada horá são 100 ints
            if(horarioAtual > 2300) { horarioAtual = horarioAtual - 2400; }
            carregarFasesDeBlitz(1);
        }
        else{
            //Debonus da organizada, demora um dia para preparar a ofensiva
            GameController.instancia.diasEmBatalha++;
            horarioAtual = 1200;
            rushado = true;
            faseDeBatalha = 3;
            showDownFinalPhase();
        }
        ajustarCentroDoTabuleiroEPosicaoSolar();        
        gerarUnidadesAliadas();
        iniciarSkybox();
        ajustarLuzDoSolAoHorario();
        InterfaceController.instancia.bloquearAteTerminarPreparacao();
        InterfaceController.instancia.tocarBGM(GameController.instancia.nodeAtual.musicaDeBackground);
        InterfaceController.instancia.mostrarHoras();
    }

    private void ajustarCentroDoTabuleiroEPosicaoSolar(){
        centroDoTabuleiro = tabuleiro[tabuleiro.Count / 2][tabuleiro[0].Count / 2].transform.position;

        rotatorDoSol.transform.position = centroDoTabuleiro;
        luzDoSol.transform.position = new Vector3(rotatorDoSol.transform.position.x, Radius, rotatorDoSol.transform.position.z);
    }

    private void iniciarSkybox(){
        componenteSkybox.material = skyboxesPossiveis[(int)GameController.instancia.nodeAtual.terreno];
    }

    private void checarBliztz(){        
        for(int i = 0 ;i < unidadesNoTabuleiro.Count; i++){
            if (unidadesNoTabuleiro[i].inimigo){
                blitzkriegBonus = false;
                return;
            }
            else if(i== aliadosEmCampo.Count){
                blitzkriegBonus = true;
                //No caso de blitz, pula 1 turno do inimigo (Pontos de ação = 0) e aumenta em 1 os pontos de ação do player
                //Na primeira rodada
                foreach(UnidadeNoTabuleiro uni in unidadesNoTabuleiro){
                    if (uni.inimigo) uni.pontosDeAcao = 0;
                    else uni.pontosDeAcao++;
                }
                return;
            }
        }
    }

    public void proximoTurno(){
        turno++;
        if (faseDeBatalha == 1 || faseDeBatalha == 2){
            if (chegadaFront == horarioAtual) TerminarFase();
        }
        if (horarioAtual != 2300) { horarioAtual += 100; }
        else { horarioAtual = 0000; GameController.instancia.diasEmBatalha++; }

        if      (!noite && (horarioAtual >= 1800 || horarioAtual <= 0600))
            noite = true;
        else if (noite && (horarioAtual < 1800 || horarioAtual > 0600))
            noite = false;
        //Feedback visual novo turno
        organizarVezesPorIniciativa();
        ajustarLuzDoSolAoHorario();
    }

    public void ajustarLuzDoSolAoHorario(){
        Light lux = luzDoSol.GetComponent<Light>();
        if (noite){
            lux.intensity = 0.75f;
			lux.color = new Color(0.31f, 0.88f, 1f);
        }
        else{
            lux.intensity = 1;
            lux.color = new Color(1f, 1f, 1f);
        }

        if(horarioAtual == 0600 || horarioAtual == 1800){
            anguloDoSol = 78;
        }
        else if (horarioAtual == 0700 || horarioAtual == 1900){
            anguloDoSol = 66;
        }
        else if (horarioAtual == 0800 || horarioAtual == 2000){
            anguloDoSol = 54;
        }
        else if (horarioAtual == 0900 || horarioAtual == 2100){
            anguloDoSol = 42;
        }
        else if (horarioAtual == 1000 || horarioAtual == 2200){
            anguloDoSol = 30;
        }
        else if (horarioAtual == 1100 || horarioAtual == 2300){
            anguloDoSol = 18;
        }
        else if (horarioAtual == 1200 || horarioAtual == 0000){
            anguloDoSol = 0;
        }
        else if (horarioAtual == 1300 || horarioAtual == 0100){
            anguloDoSol = -18;
        }
        else if (horarioAtual == 1400 || horarioAtual == 0200){
            anguloDoSol = -30;
        }
        else if (horarioAtual == 1500 || horarioAtual == 0300){
            anguloDoSol = -42;
        }
        else if (horarioAtual == 1600 || horarioAtual == 0400){
            anguloDoSol = -54;
        }
        else if (horarioAtual == 1700 || horarioAtual == 0500){
            anguloDoSol = -66;
        }
        rotatorDoSol.transform.eulerAngles = new Vector3(0, 0, anguloDoSol);
    }

    public void proximaVez(){
        //Se não e a primeira vez da fase....Tirar hihglight indicativo da unidade atualmente ativa
        if (unidadeAtualIndex != -1) { unidadesNoTabuleiro[unidadeAtualIndex].TirarHighlight(); }

        if (unidadeAtualIndex + 1 < unidadesNoTabuleiro.Count){
            unidadeAtualIndex++;
        }
        else{
            unidadeAtualIndex = 0;
            proximoTurno();
        }
        UnidadeNoTabuleiro novaUnidadeAtual = unidadesNoTabuleiro[unidadeAtualIndex];
        if (!novaUnidadeAtual.viva){ // Se a unidade ja está fora do jogo
            proximaVez();
            return;
        }
        novaUnidadeAtual.Highlight();
        StartCoroutine(CameraController.instancia.focarNaPosicaoSuave(novaUnidadeAtual.transform));

        if(novaUnidadeAtual.inimigo) { unidadesNoTabuleiro[unidadeAtualIndex].AITurno(); }
        InterfaceController.instancia.informacoesDaUnidadeAtual.popularInformacoesDaUnidadeAtual(unidadesNoTabuleiro[unidadeAtualIndex].unidadeAssociada);
        InterfaceController.instancia.terminouVez();
    }
    

    int ResultadoDaBatalha(){//5 Grande vitoria, 4 Vitoria, 3 Vitoria Apertada, 2 , 1 Grande Derrota
        if (danoTotalAliado > danoTotalInimigo + (50 * danoTotalInimigo) / 100 && numeroDeAliadosDestruidos < numeroDeInimigosDestruidos){
            vitoria = true;
            return 5;
        }
        else if (danoTotalAliado > danoTotalInimigo && numeroDeAliadosDestruidos < numeroDeInimigosDestruidos){
            vitoria = true;
            return 4;
        }
        else if (danoTotalAliado > danoTotalInimigo && numeroDeAliadosDestruidos == numeroDeInimigosDestruidos){
            vitoria = true;
            return 3;
        }
        else if (danoTotalAliado + (50 * danoTotalInimigo) / 100 < danoTotalInimigo && numeroDeAliadosDestruidos > numeroDeInimigosDestruidos){
            vitoria = false;
            return 1;
        }
        else{
            vitoria = false;
            return 2;
        }
    }

    void TerminarFase(){
        faseDeBatalha++;
        preBattle = true;
        FeedbackUnidadeAtual.gameObject.SetActive(false);
        FeedbackUnidadeAtual.transform.SetParent(this.transform);
        InterfaceController.instancia.bloquearAteTerminarPreparacao();
        if (faseDeBatalha != 4) matarTudoNaCena();
        switch (faseDeBatalha){
            case 1:
                carregarFasesDeBlitz(1);
                gerarUnidadesAliadas();
                break;
            case 2:
                if (antiSuporteBlitz.Count != 0){
                    carregarFasesDeBlitz(2);
                    gerarUnidadesAliadas();
                }
                else{//Não há nenhuma unidade de spearhead que tentou atacar o suporter inimigo, portanto pular segunda fase
                    TerminarFase();
                }
                break;
            case 3:
                showDownFinalPhase();
                gerarUnidadesAliadas();
                break;
            case 4: //Acabou a batalha
                battleGrade = ResultadoDaBatalha();
                InterfaceController.instancia.TerminarBatalha(battleGrade);
                break;
        }
    }


    public void sairDaBatalha(){//Deletar todos os assets relacionados a cena atual
        GameController.instancia.victory = vitoria;
        GameController.instancia.onBattle = false;

        SceneManager.UnloadScene("BattleScene");
    }

    public void organizarVezesPorIniciativa(){        
        unidadesNoTabuleiro.Sort((y, x) => (x.unidadeAssociada.status[2] + x.unidadeAssociada.status[3]).CompareTo((y.unidadeAssociada.status[2] + y.unidadeAssociada.status[3])));
    }

    bool faseAcabou(){
        if(faseDeBatalha == 1 && aliadosEmCampo.Count == 0){
            return true;
        }
        else if (faseDeBatalha != 1 && (aliadosEmCampo.Count == 0 || inimigosEmCampo.Count == 0)){
            return true;
        }
        return false;
    }

    public void carregarTodasAsUnidadesInimigas(){
        MapXmlContainer container;
        for (int i = 0; i < 3; i++){
            if (i == 0){
                container = MapSaveLoad.OnGameLoad(1);
                foreach (UnidadeXml uXML in container.unidadesNoTabuleiro){
                    spearheadInimigo.Add(BancoDeDados.instancia.retornarUnidade(uXML.id));
                }
            }
            else if (i == 1){
                container = MapSaveLoad.OnGameLoad(2);
                foreach (UnidadeXml uXML in container.unidadesNoTabuleiro){
                    suporteInimigo.Add(BancoDeDados.instancia.retornarUnidade(uXML.id));
                }
            }
            else if (i == 2){
                container = MapSaveLoad.OnGameLoad(3);
                foreach (UnidadeXml uXML in container.unidadesNoTabuleiro){
                    frontInimigo.Add(BancoDeDados.instancia.retornarUnidade(uXML.id));
                }
            }
        }
    }

    public List<List<Tile>> carregarTabuleiroDoContainer(MapXmlContainer container){
        colunasTabuleiro = container.coluns;
        linhasTabuleiro = container.lines;

        List<List<Tile>> tabuleiroCarregado = new List<List<Tile>>();

        for (int i = 0; i < colunasTabuleiro; i++){
            List<Tile> row = new List<Tile>();
            for (int j = 0; j < linhasTabuleiro; j++){
                Tile tile = ((GameObject)Instantiate(tileDoTabuleiroPrefab, new Vector3(i - Mathf.Floor(linhasTabuleiro / 2), 0, -j + Mathf.Floor(colunasTabuleiro / 2)), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();
                tile.posicaoNoTabuleiro = new Vector2(i, j);
                tile.transform.SetParent(tabuleiroHolder.transform);
                tile.setarTipoeVisual((TileType)container.tiles.Where(x => x.locX == i && x.locY == j).First().id);
                tile.setarMecanicaeVisual((TileMechanic)container.tiles.Where(x => x.locX == i && x.locY == j).First().mechanic);
                //Adicionar para os tiles especiais                
                if  (   (tile.posicaoNoTabuleiro.x == 0 || tile.posicaoNoTabuleiro.x == colunasTabuleiro)   ||
                        (tile.posicaoNoTabuleiro.y == 0 || tile.posicaoNoTabuleiro.y == linhasTabuleiro)    )
                    { locaisDeRetiradaInimiga.Add(tile); }
                if (tile.tipoDeMecanica == TileMechanic.ENEMY_SPAWN_SUPPORT) { spawnSuporteInimigo.Add(tile); }
                else if (tile.tipoDeMecanica == TileMechanic.ENEMY_SPAWN_REINFORCEMENT) { spawnReforcoInimigo.Add(tile); }
                else if (tile.tipoDeMecanica == TileMechanic.PLAYER_SPAWN_MAIN) { spawnAliado.Add(tile); }
                else if (tile.tipoDeMecanica == TileMechanic.PLAYER_SPAWN_REINFORCEMENT) { spawnReforco.Add(tile); }
                //Adicionar o tile criado a um row
                row.Add(tile);
            }
            //Adicionar o row ao tabuleiro
            tabuleiroCarregado.Add(row);
        }
        return tabuleiroCarregado;
    }

    public void showDownFinalPhase(){
        MapXmlContainer container = MapSaveLoad.OnGameLoad(3);

        tabuleiro = carregarTabuleiroDoContainer(container);
        //Administrar variaveis da camera
        configurarCameraParaNovoTabuleiro(tabuleiro);

        //Carregar unidades do front, que ja vem dentro do MapXmlContainer
        carregarUnidadesNoContainer(container);
        for (int j = 0; j < spearheadInimigo.Count; j++){
            if (spearheadInimigo[j].status[1] >= (spearheadInimigo[j].status[0] * 25) / 100){
                UnidadeNoTabuleiro unidadeNova = ((GameObject)Instantiate(UnidadePrefab, spawnReforcoInimigo[j].transform.position + 1 * Vector3.up, Quaternion.Euler(new Vector3(270, 90, 90)))).GetComponent<UnidadeNoTabuleiro>();
                unidadeNova.unidadeAssociada = spearheadInimigo[j];
                procurarPorArtilharias(unidadeNova);
                unidadeNova.inimigo = true;
                unidadeNova.gameObject.GetComponent<Animator>().runtimeAnimatorController = controladoresInimigos[(int)unidadeNova.unidadeAssociada.classe];
                unidadeNova.gridPosition = spawnReforcoInimigo[j].posicaoNoTabuleiro;
                unidadeNova.transform.SetParent(unidadeHolder.transform);
                spawnReforcoInimigo[j].ocupado = true;
                unidadesNoTabuleiro.Add(unidadeNova);
                inimigosEmCampo.Add(unidadeNova);
                unidadeNova.gameObject.SetActive(false);
            }
            else{
                spearheadInimigo.RemoveAt(j);
            }
        }
        for (int j = 0; j < suporteInimigo.Count; j++){
            if (suporteInimigo[j].status[1] >= (suporteInimigo[j].status[0] * 25) / 100){
                UnidadeNoTabuleiro unidadeNova = ((GameObject)Instantiate(UnidadePrefab,spawnSuporteInimigo[j].transform.position + 1 * Vector3.up, Quaternion.Euler(new Vector3(270, 90, 90)))).GetComponent<UnidadeNoTabuleiro>();
                unidadeNova.unidadeAssociada = suporteInimigo[j];
                procurarPorArtilharias(unidadeNova);
                unidadeNova.inimigo = true;
                unidadeNova.gameObject.GetComponent<Animator>().runtimeAnimatorController = controladoresInimigos[(int)unidadeNova.unidadeAssociada.classe];
                unidadeNova.gridPosition = spawnSuporteInimigo[j].posicaoNoTabuleiro;
                unidadeNova.transform.SetParent(unidadeHolder.transform);
                spawnSuporteInimigo[j].ocupado = true;
                unidadesNoTabuleiro.Add(unidadeNova);
                inimigosEmCampo.Add(unidadeNova);
                unidadeNova.gameObject.SetActive(false);
            }
            else{
                suporteInimigo.RemoveAt(j);
            }
        }
    }

    public void matarTudoNaCena(){
        if (unidadeHolder.transform.childCount != 0){
            List<GameObject> aRemover = new List<GameObject>();
            for (int i = 0; i < unidadeHolder.transform.childCount; i++){
                aRemover.Add(unidadeHolder.transform.GetChild(i).gameObject);
            }
            aRemover.ForEach(child => Destroy(child));
        }
        //Resetando variaveis das unidades
        unidadesNoTabuleiro = new List<UnidadeNoTabuleiro>();
        aliadosEmCampo = new List<UnidadeNoTabuleiro>();
        inimigosEmCampo = new List<UnidadeNoTabuleiro>();

        if (tabuleiroHolder.transform.childCount != 0){
            List<GameObject> aRemover = new List<GameObject>();
            for (int i = 0; i < tabuleiroHolder.transform.childCount; i++){
                aRemover.Add(tabuleiroHolder.transform.GetChild(i).gameObject);
            }
            aRemover.ForEach(child => Destroy(child));
        }
        //Resetando variaveis do tabulerio
        tabuleiro = new List<List<Tile>>();
        spawnAliado = new List<Tile>();
        spawnReforco = new List<Tile>();
        spawnSuporteInimigo = new List<Tile>();
        spawnReforcoInimigo = new List<Tile>();
    }

    public void carregarUnidadesNoContainer(MapXmlContainer container){
        for (int i = 0; i < container.unidadesNoTabuleiro.Count; i++){
            UnidadeXml uXML = container.unidadesNoTabuleiro[i];
            UnidadeNoTabuleiro unidadeNova = ((GameObject)Instantiate(UnidadePrefab, tabuleiro[uXML.locX][uXML.locY].transform.position + 1 * Vector3.up, Quaternion.Euler(new Vector3(270, 90, 90)))).GetComponent<UnidadeNoTabuleiro>();
            unidadeNova.gridPosition = new Vector2(uXML.locX, uXML.locY);

            if (faseDeBatalha == 1) unidadeNova.unidadeAssociada = spearheadInimigo[i];
            else if (faseDeBatalha == 2) unidadeNova.unidadeAssociada = suporteInimigo[i];
            else if (faseDeBatalha == 3) unidadeNova.unidadeAssociada = frontInimigo[i];

            procurarPorArtilharias(unidadeNova);
            unidadeNova.transform.SetParent(unidadeHolder.transform);
            unidadeNova.gameObject.GetComponent<Animator>().runtimeAnimatorController = controladoresInimigos[(int)unidadeNova.unidadeAssociada.classe];
            //Aqui so entrarão unidades que vão estar contra você no jogo, então sempre serão inimigos
            unidadeNova.inimigo = true;
            tabuleiro[uXML.locX][uXML.locY].ocupado = true;
            unidadesNoTabuleiro.Add(unidadeNova);
            inimigosEmCampo.Add(unidadeNova);
            unidadeNova.gameObject.SetActive(false);
            unidadeNova.gameObject.SetActive(false);
        }
    }

    public void carregarFasesDeBlitz(int fase){//Carregar um mapa de cada vez
        //A maior diferença entre essa função e a Showdown e que nessa a posição dos inimigos e predeterminada enquanto no Showdown elas são dinamicas
        MapXmlContainer container = MapSaveLoad.OnGameLoad(fase);

        colunasTabuleiro = container.coluns;
        linhasTabuleiro = container.lines;

        tabuleiro = carregarTabuleiroDoContainer(container);
        //Manejar eventos da camera para começar
        configurarCameraParaNovoTabuleiro(tabuleiro);

        unidadesNoTabuleiro = new List<UnidadeNoTabuleiro>();
        carregarUnidadesNoContainer(container);

        //calcularLinhaDeDefesaInicial();
        //atribuirTarefasParaAI();
    }

    public void configurarCameraParaNovoTabuleiro(List<List<Tile>> tabuleiroASerConfigurado){
        CameraController.instancia.setarTabuleiro(tabuleiroASerConfigurado);
        CameraController.instancia.calcularLimitesDaCamera();
        CameraController.instancia.olharProCentroDoTabuleiro();
    }

    public void ativarUnidadesInimigas(){
        foreach (UnidadeNoTabuleiro u in inimigosEmCampo){
            u.gameObject.SetActive(true);
        }
    }

    public IEnumerator RetiradaDaBatalha(UnidadeNoTabuleiro unidadeEmRetirada) {
        while (true) {
            CameraController.instancia.focarNoPlayer(unidadeEmRetirada);
            Color corAtual = unidadeEmRetirada.GetComponent<SpriteRenderer>().color; 
            corAtual.a -= 0.05f;
            unidadeEmRetirada.GetComponent<SpriteRenderer>().color = corAtual;
            if (corAtual.a <= 0){
                tabuleiro[(int)unidadeEmRetirada.gridPosition.x][(int)unidadeEmRetirada.gridPosition.y].ocupado = false;

                if  (unidadeEmRetirada.inimigo) {
                    inimigosEmCampo.Remove(unidadeEmRetirada);
                    if (spearheadInimigo.Contains(unidadeEmRetirada.unidadeAssociada)) spearheadInimigo.Remove(unidadeEmRetirada.unidadeAssociada);
                    else if (suporteInimigo.Contains(unidadeEmRetirada.unidadeAssociada)) suporteInimigo.Remove(unidadeEmRetirada.unidadeAssociada);
                    else if (frontInimigo.Contains(unidadeEmRetirada.unidadeAssociada)) frontInimigo.Remove(unidadeEmRetirada.unidadeAssociada);
                }
                else{
                    aliadosEmCampo.Remove(unidadeEmRetirada);
                }

                unidadesNoTabuleiro.Remove(unidadeEmRetirada);
                Destroy(unidadeEmRetirada.gameObject);
                unidadeAtualIndex--;

                if (faseAcabou()) { TerminarFase(); }
                else              { proximaVez();   }

                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator Blitzkrieg(UnidadeNoTabuleiro unidadeEmAvanco){
        while (true){
            CameraController.instancia.focarNoPlayer(unidadeEmAvanco);
            Color corAtual = unidadeEmAvanco.GetComponent<SpriteRenderer>().color;
            corAtual.a -= 15;
            unidadeEmAvanco.GetComponent<SpriteRenderer>().color = corAtual;
            if (corAtual.a <= 0) {
                tabuleiro[(int)unidadeEmAvanco.gridPosition.x][(int)unidadeEmAvanco.gridPosition.y].ocupado = false;
                aliadosEmCampo.Remove(unidadeEmAvanco);

                antiSuporteBlitz.Add(unidadeEmAvanco.unidadeAssociada);

                unidadesNoTabuleiro.Remove(unidadeEmAvanco);
                Destroy(unidadeEmAvanco.gameObject);
                unidadeAtualIndex--;

                if (faseAcabou()) { TerminarFase(); }
                else { proximaVez(); }

                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void playerAtualFezAcaoEspecial(bool aceitou){
        if (aceitou){
            TileMechanic mechanicaTriggered = tabuleiro[(int)unidadesNoTabuleiro[unidadeAtualIndex].gridPosition.x][(int)unidadesNoTabuleiro[unidadeAtualIndex].gridPosition.y].tipoDeMecanica;
            UnidadeNoTabuleiro unidadeAtual = unidadesNoTabuleiro[unidadeAtualIndex];
            switch (mechanicaTriggered)   {
                case TileMechanic.PLAYER_RETREAT_LINE:
                    StartCoroutine(RetiradaDaBatalha(unidadeAtual));
                    break;
                case TileMechanic.PLAYER_ATTACK_SUPPORT:
                    StartCoroutine(Blitzkrieg(unidadeAtual));
                    break;
            }
        }
        else{
            unidadesNoTabuleiro[unidadeAtualIndex].aguardandoPlayer = false;
            AdministrarFimDeAcao();
        }
    }

    //Highlight tiles a partir de certo ponto de origem para a cor tal até a tal distancia durante o PathFinding
    public void highlightTilesAt(Vector2 origem, Material materialDeHighlight, int distanciaDeMovimento){
        List<Tile> highlightedTiles = TileHighlight.FindHighlight(tabuleiro[(int)origem.x][(int)origem.y], distanciaDeMovimento);
        foreach (Tile t in highlightedTiles){
            t.mudarMaterial(materialDeHighlight);
        }
    }

    //Remover o highlight de todo o tabuleiro
    public void removerTileHighlights(){
        for (int i = 0; i < colunasTabuleiro; i++){
            for (int j = 0; j < linhasTabuleiro; j++){
                tabuleiro[i][j].voltarProOriginal();
            }
        }
    }

    public void verificarTentativaDeMovimento(Tile destTile){
        UnidadeNoTabuleiro atual = unidadesNoTabuleiro[unidadeAtualIndex];        
        if ((destTile.sobreHighlight && !destTile.instransponivel) || atual.inimigo){
            Vector2 posicaoAnterior = atual.gridPosition;
            Tile tileAnterior = tabuleiro[(int)posicaoAnterior.x][(int)posicaoAnterior.y];

            removerTileHighlights();
            //Desocupar o tile
            tileAnterior.ocupado = false;
            //VISUAL - Pintar o tile atual para indicar de onde a unidade vem
            tileAnterior.mudarMaterial(TileGridMovimento);
            //Achar o caminho mais curto para o destino
            List<Tile> caminhoAchado = TilePathFinder.FindPathTilesReturn(tileAnterior, destTile);;
            //Vai reduzir a quantidade de combustivel em 2 por movimento, não importando o numero de tiles
            //Isso faz as unidades moveis ainda mais vantajosas em termos de eficiencia

            atual.unidadeAssociada.status[13] -= 2;
            //Resetar a linha de tiles da Unidade, caso tenha dado algum erro ou algo parecido
            atual.linhaDeTilesaSeguir = new List<Vector3>();
            //Para cada tile no caminho achado...            
            foreach (Tile t in caminhoAchado){
                //VISUAL - pintar os tiles do caminho também
                t.mudarMaterial(TileGridMovimento);
                //Adicionar a coordenada no mundo do tile, e então adicionar um Vector.UP para ele, deixando ele acima do "solo"
                atual.linhaDeTilesaSeguir.Add(tabuleiro[(int)t.posicaoNoTabuleiro.x][(int)t.posicaoNoTabuleiro.y].transform.position + 1 * Vector3.up);
            }            
            //Atualizar logica do tabuleiro
            destTile.ocupado = true;
            atual.gridPosition = destTile.posicaoNoTabuleiro;
            //Desativar o grid de movimento, assim os proximos cliques não serão outro movimento imediatamente
            atual.gridDeMovimento = false;

            if (atual.artilharia && !atual.preparada && atual.pontosDeAcao > 0) { InterfaceController.instancia.desativarOpcaoDePosicionamento(); }
            if (atual.artilharia) { atual.preparada = false; }

            //Mover unidade visualmente, inimigos se movem com mais feedback para o player
            if (atual.inimigo) { StartCoroutine(atual.CoroutinaDeMovimentoAI()); }
            else { StartCoroutine(atual.CoroutinaDeMovimento()); }

        }
        else{
            Debug.Log("Não pode se mover para o tile clicado!");
        }
    }

    public void TerminarVez(){
        unidadesNoTabuleiro[unidadeAtualIndex].gridDeMovimento = false;
        unidadesNoTabuleiro[unidadeAtualIndex].gridDeAtaque = false;
        removerTileHighlights();
        proximaVez();
    }

    public void unidadeDeArtilhariaSePreparou(){
        UnidadeNoTabuleiro atual = unidadesNoTabuleiro[unidadeAtualIndex];
        atual.preparada = true;
        atual.pontosDeAcao = 0;
        AdministrarFimDeAcao();
    }

    public void comecarFase(){
        unidadeAtualIndex = -1;
        revelarInimigos();
        organizarVezesPorIniciativa();
        checarBliztz();
        FeedbackUnidadeAtual.gameObject.SetActive(true);        
        preBattle = false;
        proximaVez();
    }

    public void AdministrarFimDeAcao(){
        UnidadeNoTabuleiro unidadeAtual = unidadesNoTabuleiro[unidadeAtualIndex];
        unidadeAtual.pontosDeAcao--;
        if (unidadeAtual.pontosDeAcao > 0){
            InterfaceController.instancia.informacoesDaUnidadeAtual.atualizarSliders(unidadeAtual.unidadeAssociada);
            if (unidadeAtual.inimigo) { atualizarAlvosPrioritarios(); unidadeAtual.AITurno(); }
        }
        else{
            unidadeAtual.pontosDeAcao = pontosDeAcao;
            unidadeAtual.gridDeMovimento = false;
            unidadeAtual.gridDeAtaque = false;
            if (faseAcabou()) TerminarFase();
            else { proximaVez(); }            
        }
    }

    private IEnumerator CoroutinaDeAtaqueDeUnidadeDaAI(string dano, UnidadeNoTabuleiro atual, UnidadeNoTabuleiro alvo){
        atual.gridDeAtaque = true;
        //Ele começa highlighitando os nodes que a Unidade de AI pode atacar
        CameraController.instancia.focarNoPlayer(atual);
        //CameraController.instancia.setOrthopaicSize(5);
        highlightTilesAt(atual.gridPosition, TileGridAtaque, atual.unidadeAssociada.status[4]);
        yield return new WaitForSeconds(1);
        removerTileHighlights();
        Tile tileDoAlvo = tabuleiro[(int)alvo.gridPosition.x][(int)alvo.gridPosition.y];
        tileDoAlvo.mudarMaterial(TileGridAtaque);
        AudioClip temp = atual.tocarAtaque();
        yield return new WaitForSeconds(temp.length);
        //yield return new WaitForSeconds(0.5f);
        tileDoAlvo.mudarMaterial(TileGridNormal);
        CameraController.instancia.focarNoPlayer(alvo);
        //CameraController.instancia.setOrthopaicSize(3);
        alvo.GetComponent<Animator>().Play("Dano");
        criarTextoFlutuante(dano, alvo);
        //if (dano.Equals("MISS")) { alvo.tocarDano(alvo.unidadeAssociada, atual.unidadeAssociada, true); }
        //else { alvo.tocarDano(alvo.unidadeAssociada, atual.unidadeAssociada, false); }
        yield return new WaitForSeconds(alvo.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        targetFuckingDies(alvo);
        atual.gridDeAtaque = false;
        AdministrarFimDeAcao();
    }

    private IEnumerator CoroutinaDeAtaqueDeUnidade(string dano, UnidadeNoTabuleiro atual, UnidadeNoTabuleiro alvo){
        removerTileHighlights();
        Tile tileDoAlvo = tabuleiro[(int)alvo.gridPosition.x][(int)alvo.gridPosition.y];
        tileDoAlvo.mudarMaterial(TileGridAtaque);
        CameraController.instancia.focarNoPlayer(alvo);
        //CameraController.instancia.setOrthopaicSize(3);
        alvo.GetComponent<Animator>().Play("Dano");
        criarTextoFlutuante(dano, alvo);
        yield return new WaitForSeconds(alvo.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        tileDoAlvo.mudarMaterial(TileGridNormal);
        CameraController.instancia.focarNoPlayer(atual);
        targetFuckingDies(alvo);
        AdministrarFimDeAcao();
    }
    

    public void atacarComPlayerAtual(Tile destTile){
        if (destTile.sobreHighlight && !destTile.instransponivel){

            UnidadeNoTabuleiro target = null;
            foreach (UnidadeNoTabuleiro u in unidadesNoTabuleiro){
                //Checar se o alvo está no tile alvo e se ele e do tipo oposto ao do atirador e se sequer está vivo
                if (u.gridPosition == destTile.posicaoNoTabuleiro && u.inimigo != unidadesNoTabuleiro[unidadeAtualIndex].inimigo && u.viva){
                    target = u;
                }
            }
            if (target != null){
                UnidadeNoTabuleiro atual = unidadesNoTabuleiro[unidadeAtualIndex];
                if (atual.gridPosition.x >= target.gridPosition.x - atual.unidadeAssociada.status[4]  && atual.gridPosition.x <= target.gridPosition.x + atual.unidadeAssociada.status[4] &&
                    atual.gridPosition.y >= target.gridPosition.y - atual.unidadeAssociada.status[4]  && atual.gridPosition.y <= target.gridPosition.y + atual.unidadeAssociada.status[4] ){
                    atual.unidadeAssociada.status[15] -= 5;
                    atual.gridDeAtaque = false;
                    int debonusPrecisao;//A quantidade de munição sobrando influe na precisão da unidade atual
                    int debonusEsquiva;	//A quantidade de combustivel sobrando influe na esquiva do alvo

                    //Menos de 50 porcento do combustivel da um debonus de 20% da precisão/esquiva, o mesmo pra munição
                    if (atual.unidadeAssociada.status[15] < 50) debonusPrecisao = (target.unidadeAssociada.status[8] * 20) / 100;
                    else if (atual.unidadeAssociada.status[15] < 25) debonusPrecisao = (target.unidadeAssociada.status[8] * 40) / 100;
                    else debonusPrecisao = 0;

                    if (target.unidadeAssociada.status[13] < 50) debonusEsquiva = (target.unidadeAssociada.status[9] * 20) / 100;
                    else if (target.unidadeAssociada.status[13] < 25) debonusEsquiva = (target.unidadeAssociada.status[9] * 40) / 100;
                    else debonusEsquiva = 0;

                    if (noite){
                        //Diminue o debonus de esquiva para o alvo caso estiver de noite
                        debonusEsquiva -= (target.unidadeAssociada.status[9] * 50) / 100;
                        //Aumenta o debous de esquiva do atirador caso estiver de noite
                        debonusPrecisao += (atual.unidadeAssociada.status[8] * 50) / 100;
                    }
                    int esquivaFinal = target.unidadeAssociada.status[9] - debonusEsquiva;
                    int precisaoFinal = atual.unidadeAssociada.status[8] - debonusPrecisao;

                    float rollDeEsquiva = esquivaFinal + precisaoFinal;//Precisao do atual + esquiva do alvo
                    bool acertou = (int)UnityEngine.Random.Range(0, rollDeEsquiva) <= precisaoFinal;
                    if (acertou) {//HIT!
                        int valorDeAtaqueBruto = 0, danoMacio = atual.unidadeAssociada.status[5], danoPesado = atual.unidadeAssociada.status[6], danoHeroico = atual.unidadeAssociada.status[7];
                        if (target.unidadeAssociada.tipoDeAlvo == Alvos.Macio) valorDeAtaqueBruto = danoMacio + (danoPesado * 20) / 100;
                        else if (target.unidadeAssociada.tipoDeAlvo == Alvos.Duro) valorDeAtaqueBruto = danoPesado + (danoMacio * 20) / 100;
                        else if (target.unidadeAssociada.tipoDeAlvo == Alvos.Heroico) valorDeAtaqueBruto = (danoMacio * 20) / 100 + (danoPesado * 20) / 100 + danoHeroico;

                        int defesaBruta = (valorDeAtaqueBruto * target.unidadeAssociada.status[10]) / 100;
                        int bonusPerduracao = (defesaBruta * atual.unidadeAssociada.status[11]) / 100;
                        int danoPerfurante = valorDeAtaqueBruto - (defesaBruta - bonusPerduracao);

                        int danoFinal = (int)(danoPerfurante * UnityEngine.Random.Range(0.5f, 1.5f));
                        if (danoFinal <= 0) danoFinal = 1;

                        if (atual.inimigo) danoTotalInimigo += danoFinal;
                        else danoTotalAliado += danoFinal;

                        target.unidadeAssociada.status[1] -= danoFinal;
                        if (atual.inimigo) StartCoroutine(CoroutinaDeAtaqueDeUnidadeDaAI(danoFinal.ToString(), atual, target));
                        else StartCoroutine(CoroutinaDeAtaqueDeUnidade(danoFinal.ToString(), atual, target));
                    }
                    else{
                        if (atual.inimigo) StartCoroutine(CoroutinaDeAtaqueDeUnidadeDaAI("MISS", atual, target));
                        else StartCoroutine(CoroutinaDeAtaqueDeUnidade("MISS", atual, target));
                    }
                }
                else{
                    Debug.Log("Tile clicado não está no alcance dessa unidade");
                }
            }
        }
        else{
            Debug.Log("Tu clicou no nada mahn");
        }
    }

    private void criarTextoFlutuante(string texto, UnidadeNoTabuleiro alvo){
        Vector2 posicaoNaTela = Camera.main.WorldToScreenPoint(alvo.transform.position);
        FloatingText textoFlutuante = Instantiate(textoFlutuantePrefab).GetComponent<FloatingText>();
        textoFlutuante.transform.SetParent(InterfaceController.instancia.transform, false);
        textoFlutuante.transform.position = posicaoNaTela;
        textoFlutuante.setTextoDeFeedback(texto);
    }

    public void mostrarInformacoesDaUnidadeClicada(Tile tile){
        UnidadeNoTabuleiro unidade = unidadesNoTabuleiro.FirstOrDefault(uni => uni.gridPosition == tile.posicaoNoTabuleiro);
        InterfaceController.instancia.informacoesDaUnidadeAtual.popularInformacoesDaUnidadeAtual(unidade.unidadeAssociada);
    }

    public void retirarInformacoesDaUnidadeClicada(){
        UnidadeNoTabuleiro sendoMostrada = unidadesNoTabuleiro.FirstOrDefault(uni => uni.unidadeAssociada == InterfaceController.instancia.informacoesDaUnidadeAtual.unidadeSendoMostrada);
        sendoMostrada.TirarHighlight();
        InterfaceController.instancia.informacoesDaUnidadeAtual.popularInformacoesDaUnidadeAtual(unidadesNoTabuleiro[unidadeAtualIndex].unidadeAssociada);
    }

    void targetFuckingDies(UnidadeNoTabuleiro target){
        if (target.unidadeAssociada.status[1] <= 0){
            //Mudar para animação de morte
            target.unidadeAssociada.status[1] = 0;
            target.viva = false;
            target.GetComponent<Animator>().Play("Morto");
            target.GetComponent<Animator>().SetBool("Morto", true);

            target.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z + 0.25f);
            if (target.inimigo){
                inimigosEmCampo.Remove(target);
                if (spearheadInimigo.Contains(target.unidadeAssociada)) spearheadInimigo.Remove(target.unidadeAssociada);
                else if (suporteInimigo.Contains(target.unidadeAssociada)) suporteInimigo.Remove(target.unidadeAssociada);
                else if (frontInimigo.Contains(target.unidadeAssociada)) frontInimigo.Remove(target.unidadeAssociada);
                numeroDeInimigosDestruidos++;
            }
            else{
                aliadosEmCampo.Remove(target);
                removerUnidadeMortaDoPlayer(target.unidadeAssociada);
                numeroDeAliadosDestruidos++;
            }
        }
        if (faseAcabou()) TerminarFase();
    }

    private void removerUnidadeMortaDoPlayer(Unidade morta){
        if (GameController.instancia.primeiroBatalhao.Contains(morta)) GameController.instancia.primeiroBatalhao.Remove(morta);
        else if (GameController.instancia.segundoBatalhao.Contains(morta)) GameController.instancia.segundoBatalhao.Remove(morta);
        else if (GameController.instancia.terceiroBatalhao.Contains(morta)) GameController.instancia.terceiroBatalhao.Remove(morta);
    }

    public void gerarUnidadesAliadas(){
        switch (faseDeBatalha){
            case 1: //Primeira fase, portanto spawna Spearhead dos dois lados
                foreach (Unidade u in GameController.instancia.primeiroBatalhao){
                    unidadesASpawnar.Add(u);
                }
                break;
            case 2:
                //Spawnando Spearhead destinada ao Anti Suporte
                if (antiSuporteBlitz.Count != 0){
                    foreach (Unidade u in antiSuporteBlitz){
                        unidadesASpawnar.Add(u);
                    }
                }
                break;
            case 3:
                //Spawnando Spearheads que tentaram, ou não, atacar usando a Blitz
                if (rushado){//Caso não tenha sido usado a Blitz, vão spawnar em tiles normais
                    foreach (Unidade u in GameController.instancia.primeiroBatalhao) { unidadesASpawnar.Add(u); }
                }
                else{//Caso tenham, eles vão ser adicionados a uma lista que vai ser deployada em tiles especiasi
                    foreach (Unidade u in GameController.instancia.primeiroBatalhao) {
                        if (u.status[1] >= (u.status[0] * 25) / 100){
                            unidadesDoReforco.Add(u);
                        }
                    }
                }
                //Spawnando o Front do jogador
                foreach (Unidade u in GameController.instancia.segundoBatalhao) { unidadesASpawnar.Add(u); }
                //Spawnando o Suporte do jogador
                foreach (Unidade u in GameController.instancia.terceiroBatalhao) { unidadesASpawnar.Add(u); }
                break;
        }
        instanciarUnidadeAliada();
    }

    public void highlightTilesDeSpawn(bool reforco){
        if (reforco){
            foreach (Tile t in spawnReforco){
                if (!t.ocupado){
                    t.mudarMaterial(TileGridMovimento);
                    t.sobreHighlight = true;
                }
            }
        }
        else{
            foreach (Tile t in spawnAliado){
                if (!t.ocupado){
                    t.mudarMaterial(TileGridMovimento);
                    t.sobreHighlight = true;
                }
            }
        }
    }

    public void instanciarUnidadeAliada(){
        if (unidadesASpawnar.Count == 0 && unidadesDoReforco.Count == 0) {
            removerTileHighlights();
            InterfaceController.instancia.permitirTerminarPreparacao();
            return;
        }

        if (unidadeSendoSetada != null) { unidadeSendoSetada = null; }        

        UnidadeNoTabuleiro unidadeNova = ((GameObject)Instantiate(UnidadePrefab)).GetComponent<UnidadeNoTabuleiro>();
        unidadeNova.transform.position = centroDoTabuleiro + 1 * Vector3.down;
        unidadeSendoSetada = unidadeNova;

        if (faseDeBatalha == 3 && unidadesDoReforco.Count != 0){
            unidadeSendoSetada.unidadeAssociada = unidadesDoReforco[0];
            highlightTilesDeSpawn(true);
        }
        else if (unidadesASpawnar.Count != 0){
            unidadeSendoSetada.unidadeAssociada = unidadesASpawnar[0];
            highlightTilesDeSpawn(false);
        }
        /*
        else {
            removerTileHighlights();
            Destroy(unidadeSendoSetada);
            InterfaceController.instancia.permitirTerminarPreparacao();
            return;
        }*/
        InterfaceController.instancia.informacoesDaUnidadeAtual.popularInformacoesDaUnidadeAtual(unidadeSendoSetada.unidadeAssociada);
        procurarPorArtilharias(unidadeNova);
        unidadeSendoSetada.gridPosition = new Vector2(-1, -1);
        unidadeSendoSetada.inimigo = false;
        unidadeSendoSetada.gameObject.GetComponent<Animator>().runtimeAnimatorController = controladoresAliados[(int)unidadeSendoSetada.unidadeAssociada.classe];
    }

    public void reposicionarUnidadeNoTabuleiro(Tile tile){
        //Else eles não tinha sido setadas ainda, apenas mandalas embora então
        if (unidadeSendoSetada != null){
            if (unidadeSendoSetada.gridPosition == new Vector2(-1, -1)){
                Destroy(unidadeSendoSetada);
            }
            else{
                unidadeSendoSetada.transform.position = tabuleiro[(int)unidadeSendoSetada.gridPosition.x][(int)unidadeSendoSetada.gridPosition.y].transform.position + Vector3.up;
                tabuleiro[(int)unidadeSendoSetada.gridPosition.x][(int)unidadeSendoSetada.gridPosition.y].ocupado = true;
            }
        }
        UnidadeNoTabuleiro novaSelecao = aliadosEmCampo.First(x => x.gridPosition.x == tile.posicaoNoTabuleiro.x && x.gridPosition.y == tile.posicaoNoTabuleiro.y);

        unidadesNoTabuleiro.Remove(novaSelecao);
        aliadosEmCampo.Remove(novaSelecao);

        unidadeSendoSetada = novaSelecao;
        InterfaceController.instancia.informacoesDaUnidadeAtual.popularInformacoesDaUnidadeAtual(unidadeSendoSetada.unidadeAssociada);
        tile.ocupado = false;
        if (tile.tipoDeMecanica == TileMechanic.PLAYER_SPAWN_MAIN) { highlightTilesDeSpawn(false); }
        else if (tile.tipoDeMecanica == TileMechanic.PLAYER_SPAWN_REINFORCEMENT) { highlightTilesDeSpawn(true); }
    }

    public void setarUnidadeNoSlot(Tile tile){
        tile.ocupado = true;
        unidadeSendoSetada.gridPosition = tile.posicaoNoTabuleiro;
        unidadesNoTabuleiro.Add(unidadeSendoSetada);
        aliadosEmCampo.Add(unidadeSendoSetada);
        unidadeSendoSetada.transform.SetParent(unidadeHolder.transform);
        if (unidadesDoReforco.Contains(unidadeSendoSetada.unidadeAssociada)) { unidadesDoReforco.Remove(unidadeSendoSetada.unidadeAssociada); }
        else { unidadesASpawnar.Remove(unidadeSendoSetada.unidadeAssociada); }
        removerTileHighlights();
        instanciarUnidadeAliada();
    }

    public void procurarPorArtilharias(UnidadeNoTabuleiro aTestar){
        if (aTestar.unidadeAssociada.classe == Classe.Artilharia || aTestar.unidadeAssociada.classe == Classe.RocketArtilharia ||
            aTestar.unidadeAssociada.classe == Classe.AntiTank || aTestar.unidadeAssociada.classe == Classe.SPArtilharia ||
            aTestar.unidadeAssociada.classe == Classe.SPRocketArtilharia || aTestar.unidadeAssociada.classe == Classe.TankDestroyer){
            aTestar.artilharia = true;
            aTestar.preparada = false;
        }
    }

    public void revelarInimigos(){
        foreach (UnidadeNoTabuleiro u in inimigosEmCampo){
            u.gameObject.SetActive(true);
        }
    }

    public void procurarPontosDeDefesaPrioritarios(){
        for (int i = 0; i < colunasTabuleiro; i++){
            for (int j = 0; j < linhasTabuleiro; j++){
                //Pontes são fonte de interesse automaticamente
                if (tabuleiro[i][j].tipoDeTile == TileType.Ponte) { pontosDeDefesa.Add(tabuleiro[i][j]); }
                //Se estamos na fase spearhead e o tile e o de suporte inimigo
                else if (faseDeBatalha == 1 && tabuleiro[i][j].tipoDeMecanica == TileMechanic.PLAYER_ATTACK_SUPPORT) { defesaDoSuporte.Add(tabuleiro[i][j]); }
            }
        }
    }

    public void setarSaidaDaBatalha(bool qual){
        saidaDaBatalha = qual;
    }

    public void AdministrarSaidaDaBatalha(){
        if (saidaDaBatalha){
            GameController.instancia.victory = false;
            GameController.instancia.onBattle = false;

            SceneManager.UnloadScene("BattleScene");
        }
        else{
            Destroy(GameController.instancia.gameObject);
            SceneManager.LoadScene("Menu");
        }
    }

    /*public void calcularLinhaDeDefesaInicial(){
        if (tipoDeIA == 1 && defesaDoSuporte.Count != 0){
            linhaDeDefesa = (int)defesaDoSuporte.Min(tile => tile.posicaoNoTabuleiro.x);
        }
        else if (tipoDeIA == 2 && posicoesDeReforco.Count != 0){
            linhaDeDefesa = (int)posicoesDeReforco.Min(tile => tile.posicaoNoTabuleiro.x);
        }
        else if (tipoDeIA == 3){
            linhaDeDefesa = (int)spawnAliado.Min(tile => tile.posicaoNoTabuleiro.x);
        }
        Debug.Log("Linhas de defesa projetada para a coluna " + linhaDeDefesa);
    }*/

    public void atualizarAlvosPrioritarios(){
        alvosPrioritarios = new List<UnidadeNoTabuleiro>();
        foreach (UnidadeNoTabuleiro uni in aliadosEmCampo){
            if (uni.gridPosition.x > linhaDeDefesa){
                alvosPrioritarios.Add(uni);
            }
        }
    }
    /*
    public bool[] cercadoPorTilesDificeis(Tile tile){
        //Não vale apena ir pra esse tile porque ele também e dificil
        if(tile.custoDeMovimento>2) {return null;}
        //Variaveis que vão verificar os tiles ao redor desse para saber quais se são dificeis de passar
        bool topoDificil = false;
        bool baixoDificil = false;
        bool direitaDificil = false;
        bool esquerdaDificil = false;
        bool diagonalTopoDireita = false;
        bool diagonalTopoEsquerdo = false;
        bool diagonalBaixoDireito = false;
        bool diagonalBaixoEsquerdo = false;
        //variveis pra determinar onde está esse tile 
        bool cantos = (tile.posicaoNoTabuleiro == new Vector2(0,0)) || (tile.posicaoNoTabuleiro == new Vector2(colunasTabuleiro-1, 0))
            || (tile.posicaoNoTabuleiro == new Vector2(0, linhasTabuleiro-1)) || (tile.posicaoNoTabuleiro == new Vector2(colunasTabuleiro - 1, linhasTabuleiro - 1));
        bool bordas = !cantos && (tile.posicaoNoTabuleiro.x >= 0 || tile.posicaoNoTabuleiro.x <= colunasTabuleiro - 1
            );
        int unidadesPossiveisParaMontarLinha = 0;
        //Quantas unidades tem para fazer uma linha de defesa nessa fase
        if (inimigosEmCampo.Count < 3) { unidadesPossiveisParaMontarLinha = inimigosEmCampo.Count; }
        else { unidadesPossiveisParaMontarLinha = inimigosEmCampo.Count / 2; }

        //Checar, na vertical, se esse tile está entre dois tiles instransponiveis ou dificeis de passar
        //Checar, na horizontal, se esse tile está entre dois tiles instransponiveis ou dificieis de passar
        //Checar, na diagonal Topo-Direita & Baixo-Esquerdo se se esse tile está entre dois tiles instransponiveis ou dificieis de passar
        
    }
    */
}

