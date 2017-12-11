using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Assets.Logica.Classes;
using System.Linq;
using System;

public class UnidadeNoTabuleiro : MonoBehaviour{
    public Vector2 gridPosition = Vector2.zero;
    public Unidade unidadeAssociada;
    public bool inimigo;
    public Vector3 destinoDeMovimento;
    public int velocidade = 4;//Velocidade de movimento que o gameobject se movimenta

    public int pontosDeAcao = 2;//Quantas ações a unidade pode fazer no seu turno(Mover ou atacar)
    public PapeisDaIA papelDessaUnidade = PapeisDaIA.Indeterminado;//Papel dessa IA no jogo, se for player normal desconsiderar

    public bool gridDeMovimento = false;//Se o player está atualmente fazendo essa unidade se mover
    public bool gridDeAtaque = false;//Se o player está atualmente usando essa unidade para atacar
    public bool viva = true;//Se essa unidade está com HP maior que 0
    public bool preparada = false;//No caso de unidades que precisam se preparar antes de atacar, essa variavel e falsa
    public bool artilharia = false;//Se essa unidade e do tipo artilharia, que nescessita se posicionar antes de começar a atirar
    public int turnosParaPreparar=0;
    public bool aguardandoPlayer = false;//Se está esperando o player decidir uma ação comrespodente a um tile especial
    public bool AIfazAcaoEspecial = false;//Se a ai está querendo fazer uma ação especial
    public AudioSource fonteDeSomDaUnidade;

    private UnidadeNoTabuleiro alvoDeAtaque;
    //Para impedir que as unidades da AI backtrackam, salvar o ultimo caminho e evitar que o tile de destino seja um deles
    //public List<Tile> ultimoCaminhoUsado;
    public List<Vector3> linhaDeTilesaSeguir = new List<Vector3>();//Para se mover através dos tiles no estilo FF tem que ter uma lista com a ordem a ser seguida

    void Awake(){
        destinoDeMovimento = transform.position;
    }

    public void Highlight(){
        if (!BatalhaController.instancia.FeedbackUnidadeAtual.GetComponent<Animator>().enabled)
            BatalhaController.instancia.FeedbackUnidadeAtual.GetComponent<Animator>().enabled = true;
        BatalhaController.instancia.FeedbackUnidadeAtual.transform.SetParent(this.transform);
        BatalhaController.instancia.FeedbackUnidadeAtual.transform.position = this.transform.position - 0.5f * Vector3.up;
        if (inimigo) { GetComponent<SpriteRenderer>().color = new Color32(225, 51, 156, 255); }
        else         { GetComponent<SpriteRenderer>().color = new Color32(179, 255, 107 ,255); }
    }

    public void TirarHighlight(){
        GetComponent<SpriteRenderer>().color = Color.white;
    }   

    public IEnumerator CoroutinaDeMovimento(){
        while (linhaDeTilesaSeguir.Count > 0){
            transform.position += (linhaDeTilesaSeguir[0] - transform.position).normalized * velocidade * Time.deltaTime;
            BatalhaController.instancia.FeedbackUnidadeAtual.transform.position = this.transform.position - 0.5f * Vector3.up;
            if (inimigo) { GetComponent<SpriteRenderer>().color = new Color32(225, 51, 156, 255); }
            CameraController.instancia.focarNoPlayer(this);
            if (Vector3.Distance(linhaDeTilesaSeguir[0], transform.position) <= 0.1f){
                transform.position = linhaDeTilesaSeguir[0];
                linhaDeTilesaSeguir.RemoveAt(0);
                if (linhaDeTilesaSeguir.Count == 0){
                    BatalhaController.instancia.removerTileHighlights();
                    Tile tileAtual = BatalhaController.instancia.tabuleiro[(int)gridPosition.x][(int)gridPosition.y];
                    if (tileAtual.ataqueDoSuporte || (tileAtual.pontoDeInicio && unidadeAssociada.status[1] <= (unidadeAssociada.status[0] * 25 / 100))){
                        aguardandoPlayer = true;
                        InterfaceController.instancia.mostrarPromptTileEspecial(tileAtual.tipoDeMecanica);
                        yield break;
                    }
                    BatalhaController.instancia.AdministrarFimDeAcao();
                    yield break;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator CoroutinaDeMovimentoAI(){
        yield return new WaitForSeconds(1.0f);
        while (linhaDeTilesaSeguir.Count > 0){
            transform.position += (linhaDeTilesaSeguir[0] - transform.position).normalized * velocidade * Time.deltaTime;
            CameraController.instancia.focarNoPlayer(this);
            if (Vector3.Distance(linhaDeTilesaSeguir[0], transform.position) <= 0.1f){
                transform.position = linhaDeTilesaSeguir[0];
                linhaDeTilesaSeguir.RemoveAt(0);
                if (linhaDeTilesaSeguir.Count == 0){
                    BatalhaController.instancia.removerTileHighlights();
                    Tile tileAtual = BatalhaController.instancia.tabuleiro[(int)gridPosition.x][(int)gridPosition.y];
                    if (BatalhaController.instancia.locaisDeRetiradaInimiga.Contains(tileAtual) && AIfazAcaoEspecial){
                        StartCoroutine(BatalhaController.instancia.RetiradaDaBatalha(this));
                        AIfazAcaoEspecial = false;
                        yield break;
                    }
                    else{
                        BatalhaController.instancia.AdministrarFimDeAcao();
                        yield break;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    //A Partir daqui os codigos da IA
    public void AITurno(){
        if (!viva ) {
            Debug.Log("Algo estranho, unidade morta teve turno chamado");
            return;
        }//Seja lá o que aconteceu, uma unidade morta não faz ação

        //Caso sofra do efeito blitzkrieg, a unidade inimiga perderá a vez
        if (pontosDeAcao == 0 && !AIfazAcaoEspecial) {
            BatalhaController.instancia.AdministrarFimDeAcao(); }
        //Novo turno = novo alvo, e portanto, o caminho pode ser que seja nescessario backtrackar
        if (pontosDeAcao == 2) { alvoDeAtaque = null;}

        DecidirAcao();
    }
    //Essa unidade está procurando alguém para entrar em confronto
    public void DecidirAcao(){
        //List<UnidadeNoTabuleiro> unidadesNoCampo = BatalhaController.instancia.unidadesNoTabuleiro;
        //List<UnidadeNoTabuleiro> alvosPrioritarios = BatalhaController.instancia.alvosPrioritarios;
        List<List<Tile>> tabuleiro = BatalhaController.instancia.tabuleiro;
        Tile tileAtual = tabuleiro[(int)gridPosition.x][(int)gridPosition.y];

        //Unidade está com problemas, retirar da batalha pra evitar mais casualidades
        if (unidadeAssociada.status[13] <= 10 || unidadeAssociada.status[15] <= 10
            || unidadeAssociada.status[1] <= (unidadeAssociada.status[0] * 25) / 100){            
            Tile tileARetirar = retiradaDaBatalhaDaAI(tileAtual);
            AIfazAcaoEspecial = true;
            if (tileARetirar != tileAtual)
                BatalhaController.instancia.verificarTentativaDeMovimento(tileARetirar);
            else
                StartCoroutine(BatalhaController.instancia.RetiradaDaBatalha(this));
            return;
        }
        AIfazAcaoEspecial = false;

        if (alvoDeAtaque==null || !alvoDeAtaque.viva){
            if(unidadeAssociada.status[4] > 1)
                alvoDeAtaque = ProcurarMaisProximoInimigo();
            else
                alvoDeAtaque = ProcurarMaisProximoInimigoDeCloseCombat();
        }
        if (artilharia){artilleryMove(alvoDeAtaque, tileAtual);}
        else{TrackDownEnemies(alvoDeAtaque, tileAtual);}        
        //A IA vai sempre atrás de alvos prioritarios, mesmo se eles estiverem longe da linha de defesa
        /*if(alvosPrioritarios.Count != 0 && unidadesNoLimite.Intersect(alvosPrioritarios).Any()){
            List<UnidadeNoTabuleiro> alvosPrioritariosEmRange = unidadesNoLimite.Intersect(alvosPrioritarios).ToList();
            TrackDownEnemies(alvosPrioritarios.First(uni => uni.gridPosition.x == alvosPrioritarios.Max(coluna => coluna.gridPosition.x)),tileAtual);
        }
        //Sem nenhum alvo prioritario a AI vai verificar se está muito longe da linha defensiva
        else if (gridPosition.x > colunaDeDefesa+5 || gridPosition.x < colunaDeDefesa-5){
            Debug.Log("Estamos fora da linha defensiva!");
            ReestruturarLinhaDefensiva(colunaDeDefesa,tileAtual);
        }
        //Estando na linha defensiva e sem nennhum alvo prioritario, ela vai atrás de atacar alguém
        else{
            //Pegar o alvo com a menor distancia para com essa unidade de IA, o foco e atacar 2 vezes sempre que possivel
            UnidadeNoTabuleiro alvoMaisProximo = unidadesNoLimite.FirstOrDefault(uni => Vector2.Distance(tileAtual.posicaoNoTabuleiro, new Vector2(uni.gridPosition.x, uni.gridPosition.y)) == unidadesNoLimite.Min(min => Vector2.Distance(tileAtual.posicaoNoTabuleiro, new Vector2(min.gridPosition.x, min.gridPosition.y))));
            //Se tiver um alvo qualquer em Range
            if (alvoMaisProximo!=null) { TrackDownEnemies(alvoMaisProximo,tileAtual); }
            //Nenehum inimigo na area de influencia, ja está na linha de defesa, pular turno
            else {
                Debug.Log("Nada pra AI fazer, proximo");
                BatalhaController.instancia.proximaVez();
            }
        }
        */
    }

    //Diferente das unidades de mais de 1 tiles de Range, close combat units não devem ir atrás de um inimigo que ja está cercado
    public UnidadeNoTabuleiro ProcurarMaisProximoInimigoDeCloseCombat(){        
        List<UnidadeNoTabuleiro> inimigosNoTabuleiro = BatalhaController.instancia.aliadosEmCampo;
        while (true){
            float menorDistancia = inimigosNoTabuleiro.Min(uni => Vector2.Distance(gridPosition, uni.gridPosition));
            UnidadeNoTabuleiro alvo = inimigosNoTabuleiro.First(uni => Vector2.Distance(gridPosition, uni.gridPosition) == menorDistancia);
            Tile tileDoAlvo = BatalhaController.instancia.tabuleiro[(int)alvo.gridPosition.x][(int)alvo.gridPosition.y];
            foreach(Tile t in tileDoAlvo.vizinhos){
                if (!t.ocupado){
                    return alvo;
                }
            }
            inimigosNoTabuleiro.Remove(alvo);
        }
    }

    public UnidadeNoTabuleiro ProcurarMaisProximoInimigo(){
        List<UnidadeNoTabuleiro> inimigosNoTabuleiro = BatalhaController.instancia.aliadosEmCampo;
        float menorDistancia = inimigosNoTabuleiro.Min(uni => Vector2.Distance(gridPosition, uni.gridPosition));
        UnidadeNoTabuleiro alvo = inimigosNoTabuleiro.First(uni => Vector2.Distance(gridPosition, uni.gridPosition) == menorDistancia);
        return alvo;
    }
    

    public void TrackDownEnemies(UnidadeNoTabuleiro target, Tile tileDeOrigem){
        Tile tileDoTarget = BatalhaController.instancia.tabuleiro[(int)target.gridPosition.x][(int)target.gridPosition.y];
        //Primeiro Caso, Alvo em Range e não precisa se mover a ele, portanto double attack
        gridDeAtaque=true;
        gridDeMovimento= false;

        if (TileHighlight.FindHighlight(tileDeOrigem, unidadeAssociada.status[4]).Contains(tileDoTarget)){
            BatalhaController.instancia.highlightTilesAt(tileDeOrigem.posicaoNoTabuleiro,BatalhaController.instancia.TileGridAtaque,unidadeAssociada.status[4]);
            BatalhaController.instancia.atacarComPlayerAtual(tileDoTarget);
        }        
        //Segundo caso, Alvo não está em range, mas pode ser alcançado com um movimento
        else{
            gridDeAtaque = false;
            gridDeMovimento = true;
            BatalhaController.instancia.highlightTilesAt(tileDeOrigem.posicaoNoTabuleiro,BatalhaController.instancia.TileGridMovimento,unidadeAssociada.status[3]);
            //Pegar todos os tiles de movimento possivel
            List<Tile> movementTiles = TileHighlight.FindHighlight(tileDeOrigem,unidadeAssociada.status[3]);
            //Verificar se algums dos tiles está a uma distancia que a unidade pode atacar após mover
            if (movementTiles.Any(tile => Vector2.Distance(tile.posicaoNoTabuleiro,tileDoTarget.posicaoNoTabuleiro)==unidadeAssociada.status[4])) {
                //Pegar qualquer tile cuja distancia para o alvo seja igual ao range maximo do seu ataque
                //List<Tile> possiveisParaMovimento = movementTiles.Where(tile => Vector2.Distance(tile.posicaoNoTabuleiro, tileDoTarget.posicaoNoTabuleiro) == unidadeAssociada.status[4]).ToList();
                //float maiorDistancia = movementTiles.Max(tile => Vector2.Distance(tileDoTarget.posicaoNoTabuleiro, tile.posicaoNoTabuleiro));
                Tile tileParaMoverMaisProximo = movementTiles.First( tile => Vector2.Distance(tile.posicaoNoTabuleiro,tileDoTarget.posicaoNoTabuleiro) == unidadeAssociada.status[4]);
                BatalhaController.instancia.verificarTentativaDeMovimento(tileParaMoverMaisProximo);
                alvoDeAtaque = target;           
            }
             //Terceiro caso, Muito longe para atacar o inimigo depois do movimento, porntanto so se importar com chegar o mais proximo possivel
            else{
                List<Tile> CaminhoMaisCurtoParaOAlvo = null;
                foreach(Tile tile in tileDoTarget.vizinhos){
                    if (!tile.ocupado) { CaminhoMaisCurtoParaOAlvo = TilePathFinder.FindPathTilesReturn(tileDeOrigem, tile); break; }
                }

                if(CaminhoMaisCurtoParaOAlvo == null) {
                    BatalhaController.instancia.proximaVez();
                    return;
                }             

                for(int i = CaminhoMaisCurtoParaOAlvo.Count - 1; i >= 0; i--){
                    if (movementTiles.Contains(CaminhoMaisCurtoParaOAlvo[i])){
                        BatalhaController.instancia.verificarTentativaDeMovimento(CaminhoMaisCurtoParaOAlvo[i]);
                        break;
                    }
                }
                //movementTiles.Any(tile => CaminhoMaisCurtoParaOAlvo.Contains(tile));   
            }
            gridDeMovimento = false;
        }
    }

    public void artilleryMove(UnidadeNoTabuleiro target, Tile tileDeOrigem){
        Tile tileDoTarget = BatalhaController.instancia.tabuleiro[(int)target.gridPosition.x][(int)target.gridPosition.y];
        //Primeiro Caso, Alvo em Range e não precisa se mover a ele, portanto double attack
        gridDeAtaque = true;
        gridDeMovimento = false;

        if (TileHighlight.FindHighlight(tileDeOrigem, unidadeAssociada.status[4]).Contains(tileDoTarget)){
            if (preparada){
                BatalhaController.instancia.highlightTilesAt(tileDeOrigem.posicaoNoTabuleiro, BatalhaController.instancia.TileGridAtaque, unidadeAssociada.status[4]);
                BatalhaController.instancia.atacarComPlayerAtual(tileDoTarget);
            }
            else{
                BatalhaController.instancia.unidadeDeArtilhariaSePreparou();
            }            
        }
        else{
            gridDeAtaque = false;
            gridDeMovimento = true;
            Classe classeDaUnidade = unidadeAssociada.classe;

            if (!preparada){
                BatalhaController.instancia.unidadeDeArtilhariaSePreparou();
            }
            else{
                BatalhaController.instancia.AdministrarFimDeAcao();
            }
            gridDeMovimento = false;
        }
    }

    private Tile fugirDe(Tile tileASeAfastar, Tile tileDeOrigem){
        List<Tile> tilesEmDistancia = TileHighlight.FindHighlight(tileDeOrigem, unidadeAssociada.status[3]);
        float distanciaMaxima = tilesEmDistancia.Max(tile => Vector2.Distance(tile.posicaoNoTabuleiro, tileASeAfastar.posicaoNoTabuleiro));
        return tilesEmDistancia.First(tile => Vector2.Distance(tile.posicaoNoTabuleiro, tileASeAfastar.posicaoNoTabuleiro) == distanciaMaxima);
    }

    private Tile retiradaDaBatalhaDaAI(Tile tileDeOrigem){
        if (tileDeOrigem.vizinhos.Count != 4){
            return tileDeOrigem;
        }

        List<TilePath> pathsParaRetirada = new List<TilePath>();
        List<Tile> movementTiles = TileHighlight.FindHighlight(tileDeOrigem, unidadeAssociada.status[3]);

        foreach (Tile tilePossivelDeRetirar in BatalhaController.instancia.locaisDeRetiradaInimiga.Where(tile => movementTiles.Contains(tile))){           
            pathsParaRetirada.Add(TilePathFinder.FindPathTilePathReturn(tileDeOrigem, tilePossivelDeRetirar));
        }

        List<Tile> caminhoEscolhido = pathsParaRetirada.First(tilePath => tilePath.custoDoCaminho == pathsParaRetirada.Min(custos => custos.custoDoCaminho)).listaDeTiles;
        
        for (int i = caminhoEscolhido.Count - 1; i >= 0; i--){
            if (movementTiles.Contains(caminhoEscolhido[i])){
                return caminhoEscolhido[i];
            }
        }
        return null;
    }

    /*public void ReestruturarLinhaDefensiva(int linhaDeDefesa, Tile origem){
        Debug.Log("Unidade " + unidadeAssociada.nome + " está Reestruturando a linha de defesa!");
        //Começando por pegar todos os tiles em range da unidade
        InterfaceController.instancia.ModoDeMovimento();
        List<Tile> inRange = TileHighlight.FindHighlight(origem, unidadeAssociada.status[3] / 2);
        //Depois disso, verificar quais dos tiles no range estão dentro da area da linha de defesa
        List<Tile> alvosPossiveis = inRange.Where(tile => tile.posicaoNoTabuleiro.x > linhaDeDefesa-5 || tile.posicaoNoTabuleiro.x > linhaDeDefesa + 5).ToList();
        //Se a lista acima voltar vazia e porque a unidade deve se mover o maximo que poder para mais proximo do centro da linha de defesa
        if(alvosPossiveis.Count == 0){
            Debug.Log("Muito longe da linha, voltando o mais proximo dela");
            Tile tileCentral = BatalhaController.instancia.tabuleiro[BatalhaController.instancia.colunasTabuleiro / 2][BatalhaController.instancia.linhasTabuleiro / 2];
            float menorDistancia = inRange.Min(tile => Vector2.Distance(tile.posicaoNoTabuleiro, tileCentral.posicaoNoTabuleiro));
            Tile tileParaMoverMaisProximo = inRange.First(tile => Vector2.Distance(tile.posicaoNoTabuleiro, tileCentral.posicaoNoTabuleiro) == menorDistancia);
            BatalhaController.instancia.verificarTentativaDeMovimento(tileParaMoverMaisProximo);
        }
        //Se tem somente um, não e nescessario mais continuar a filtrar as escolhas
        else if (alvosPossiveis.Count == 1){
            Debug.Log("Longe da linha, movendo para ela");
            BatalhaController.instancia.verificarTentativaDeMovimento(alvosPossiveis.First());
        }
        //Caso tenha mais de uma, vamos filtrar para a coluna mais a esquerda, portanto a mais longe do centro da linha
        else{
            Debug.Log("Longe da linha, mais de uma escolha");
            List<Tile> colunaFinal = alvosPossiveis.Where(tile => tile.posicaoNoTabuleiro.x == alvosPossiveis.Min(x => x.posicaoNoTabuleiro.x)).ToList();
            //Se depois de filtrarmos pela colunas mais a esquerda sobrar apenas um, mais uma vez ja sabemos o que fazer
            if (colunaFinal.Count == 1){
                Debug.Log("Apenas uma escolha final");
                BatalhaController.instancia.verificarTentativaDeMovimento(alvosPossiveis.First());
            }
            //Finalmente, vamos tentar ir o mais pro centro possivel na nossa escolha
            else{
                //Pegar a coordenada da linha do meio da tabuleiro
                Debug.Log("Mais de uma escolha final, pegando o tile mais proximo do centro");
                int meio = BatalhaController.instancia.linhasTabuleiro / 2;
                Tile tileFinal = colunaFinal.Where(tile => tile.posicaoNoTabuleiro.y == colunaFinal.Min(x => Math.Abs(x.posicaoNoTabuleiro.y - meio))).First();
                Debug.Log("Centro e " + meio + " tile final fica na linha "+tileFinal.posicaoNoTabuleiro.y);
                BatalhaController.instancia.verificarTentativaDeMovimento(tileFinal);
            }
        }
        
    }*/
    /*
    public void InterceptarInimigo(List<Tile> attacktilesInRange, List<Tile> movementToAttackTilesInRange, List<Tile> movementTilesInRange){
        Debug.Log("InterpectarInimigo chamado");
        Debug.Log(attacktilesInRange.Where(x => BatalhaController.instancia.unidadesNoTabuleiro.Where(y => !y.inimigo && y.unidadeAssociada.status[1] > 0 && y != this && y.gridPosition == x.posicaoNoTabuleiro).Count() > 0).Count().ToString());
        Debug.Log(movementToAttackTilesInRange.Where(x => BatalhaController.instancia.unidadesNoTabuleiro.Where(y => !y.inimigo && y.unidadeAssociada.status[1] > 0 && y != this && y.gridPosition == x.posicaoNoTabuleiro).Count() > 0).Count().ToString());
        Debug.Log(movementTilesInRange.Where(x => BatalhaController.instancia.unidadesNoTabuleiro.Where(y => !y.inimigo && y.unidadeAssociada.status[1] > 0 && y != this && y.gridPosition == x.posicaoNoTabuleiro).Count() > 0).Count().ToString());
        
        //Alvos prioritarios devem ser atacados imediatamente pelos enemy hunter pois passaram da linha de defesa
        bool inimigoPrioritarioExiste = BatalhaController.instancia.alvosPrioritarios.Count != 0;
        //attack if in range and with lowest HP
        if (attacktilesInRange.Where(x => BatalhaController.instancia.unidadesNoTabuleiro.Where(y => !y.inimigo && y.unidadeAssociada.status[1] > 0 && y != this && y.gridPosition == x.posicaoNoTabuleiro).Count() > 0).Count() > 0){
            List<UnidadeNoTabuleiro> opponentsInRange = attacktilesInRange.Select(x => BatalhaController.instancia.unidadesNoTabuleiro.Where(y => !y.inimigo && y.unidadeAssociada.status[1] > 0 && y != this && y.gridPosition == x.posicaoNoTabuleiro).Count() > 0 ? BatalhaController.instancia.unidadesNoTabuleiro.Where(y => y.gridPosition == x.posicaoNoTabuleiro).First() : null).ToList();

            UnidadeNoTabuleiro opponent = null;
            if (inimigoPrioritarioExiste) opponent = opponentsInRange.FirstOrDefault(x => BatalhaController.instancia.alvosPrioritarios.Contains(x));
            if (opponent== null) opponent = opponentsInRange.OrderBy(x => x.gridPosition.x < BatalhaController.instancia.linhaDeDefesa-3).ThenBy(x => x != null ? -x.unidadeAssociada.status[1] : 1000).First();

            Debug.Log("Atacando " + opponent.name + " no tile " + opponent.gridPosition);
            BatalhaController.instancia.removerTileHighlights();
            movendo = false;
            atacando = true;
            BatalhaController.instancia.highlightTilesAt(gridPosition, BatalhaController.instancia.TileGridAtaque, unidadeAssociada.status[4] / 2);

            BatalhaController.instancia.atacarComPlayerAtual(BatalhaController.instancia.tabuleiro[(int)opponent.gridPosition.x][(int)opponent.gridPosition.y]);
        }
        //move toward nearest attack range of opponent
        else if (!movendo && movementToAttackTilesInRange.Where(x => BatalhaController.instancia.unidadesNoTabuleiro.Where(y => !y.inimigo && y.unidadeAssociada.status[1] > 0 && y != this && y.gridPosition == x.posicaoNoTabuleiro).Count() > 0).Count() > 0){
            List<UnidadeNoTabuleiro> opponentsInRange = movementToAttackTilesInRange.Select(x => BatalhaController.instancia.unidadesNoTabuleiro.Where(y => !y.inimigo && y.unidadeAssociada.status[1] > 0 && y != this && y.gridPosition == x.posicaoNoTabuleiro).Count() > 0 ? BatalhaController.instancia.unidadesNoTabuleiro.Where(y => y.gridPosition == x.posicaoNoTabuleiro).First() : null).ToList();
            UnidadeNoTabuleiro opponent = opponentsInRange.OrderBy(x => x != null ? -x.unidadeAssociada.status[1] : 1000).ThenBy(x => x != null ? TilePathFinder.FindPath(BatalhaController.instancia.tabuleiro[(int)gridPosition.x][(int)gridPosition.y], BatalhaController.instancia.tabuleiro[(int)x.gridPosition.x][(int)x.gridPosition.y]).Count() : 1000).First();

            Debug.Log("Mover e atacar");
            BatalhaController.instancia.removerTileHighlights();
            movendo = true;
            atacando = false;
            BatalhaController.instancia.highlightTilesAt(gridPosition, BatalhaController.instancia.TileGridMovimento, unidadeAssociada.status[3] / 2);

            List<Tile> path = TilePathFinder.FindPath(BatalhaController.instancia.tabuleiro[(int)gridPosition.x][(int)gridPosition.y], BatalhaController.instancia.tabuleiro[(int)opponent.gridPosition.x][(int)opponent.gridPosition.y]);
            //List<Tile> path = TilePathFinder.FindPath(BatalhaController.instancia.tabuleiro[(int)gridPosition.x][(int)gridPosition.y], BatalhaController.instancia.tabuleiro[(int)opponent.gridPosition.x][(int)opponent.gridPosition.y], BatalhaController.instancia.unidadesNoTabuleiro.Where(x => x.gridPosition != gridPosition && x.gridPosition != opponent.gridPosition).Select(x => x.gridPosition).ToArray());
            BatalhaController.instancia.moverUnidadeAtual(path[(int)Mathf.Max(0, path.Count - 1 - unidadeAssociada.status[4] / 2)]);
        }
        //move toward nearest opponent        
        else if (!movendo && movementTilesInRange.Where(x => BatalhaController.instancia.unidadesNoTabuleiro.Where(y => !y.inimigo && y.unidadeAssociada.status[1] > 0 && y != this && y.gridPosition == x.posicaoNoTabuleiro).Count() > 0).Count() > 0) {
            Debug.Log("Mover e mover");
            List<UnidadeNoTabuleiro> opponentsInRange = movementTilesInRange.Select(x => BatalhaController.instancia.unidadesNoTabuleiro.Where(y => !y.inimigo && y.unidadeAssociada.status[1] > 0 && y != this && y.gridPosition == x.posicaoNoTabuleiro).Count() > 0 ? BatalhaController.instancia.unidadesNoTabuleiro.Where(y => y.gridPosition == x.posicaoNoTabuleiro).First() : null).ToList();
            UnidadeNoTabuleiro opponent = opponentsInRange.OrderBy(x => x != null ? -x.unidadeAssociada.status[1] : 1000).ThenBy(x => x != null ? TilePathFinder.FindPath(BatalhaController.instancia.tabuleiro[(int)gridPosition.x][(int)gridPosition.y], BatalhaController.instancia.tabuleiro[(int)x.gridPosition.x][(int)x.gridPosition.y]).Count() : 1000).First();

            BatalhaController.instancia.removerTileHighlights();
            movendo = true;
            atacando = false;
            BatalhaController.instancia.highlightTilesAt(gridPosition, BatalhaController.instancia.TileGridMovimento, unidadeAssociada.status[3] / 2);

            List<Tile> path = TilePathFinder.FindPath(BatalhaController.instancia.tabuleiro[(int)gridPosition.x][(int)gridPosition.y], BatalhaController.instancia.tabuleiro[(int)opponent.gridPosition.x][(int)opponent.gridPosition.y]);
            //List<Tile> path = TilePathFinder.FindPath(BatalhaController.instancia.tabuleiro[(int)gridPosition.x][(int)gridPosition.y], BatalhaController.instancia.tabuleiro[(int)opponent.gridPosition.x][(int)opponent.gridPosition.y], BatalhaController.instancia.unidadesNoTabuleiro.Where(x => x.gridPosition != gridPosition && x.gridPosition != opponent.gridPosition).Select(x => x.gridPosition).ToArray());
            BatalhaController.instancia.moverUnidadeAtual(path[(int)Mathf.Min(Mathf.Max(path.Count - 1 - 1, 0), unidadeAssociada.status[3] / 2 - 1)]);
        }
    }
    */


    //-----------------------------------Audio-------------------------------
    public AudioClip tocarAtaque(){
        fonteDeSomDaUnidade.Stop();
        AudioClip soundToPlay = null;

        switch (unidadeAssociada.tipoDeSom){
            case SonsDeAtaque.CROSSBOW:
                soundToPlay = SoundHolder.instancia.Crossbow;
                break;
            case SonsDeAtaque.INFATARIA_MEDIEVAL:
                soundToPlay = SoundHolder.instancia.MedievalInfantry;
                break;
            case SonsDeAtaque.CAVALARIA_MEDIEVAL:
                soundToPlay = SoundHolder.instancia.CavalaryCharge;
                break;
            case SonsDeAtaque.ARMA_DE_CERCO_MEDIEVAL:
                soundToPlay = SoundHolder.instancia.ArmaDeCercoMedieval;
                break;
            case SonsDeAtaque.WW2_SHOOTS:
                soundToPlay = SoundHolder.instancia.RifleShootingGunfight;
                break;
            case SonsDeAtaque.WW2_SNIPER_SHOOT:
                soundToPlay = SoundHolder.instancia.SniperQuickScope;
                break;
            case SonsDeAtaque.WW2_TANKS:
                soundToPlay = SoundHolder.instancia.TankTurrerStrike;
                break;
            case SonsDeAtaque.FOG_LORD:
                soundToPlay = SoundHolder.instancia.FogLordAttack;
                break;
        }
        fonteDeSomDaUnidade.PlayOneShot(soundToPlay);
        return soundToPlay;
    }
    /*
    public AudioClip tocarDano(Unidade unidadeAlvo, Unidade unidadeAtacando, bool miss) {
        fonteDeSomDaUnidade.Stop();
        AudioClip soundToPlay = null;

        if (unidadeAlvo.tipoDeAlvo == Alvos.Duro){
            if (miss) {
                if(unidadeAtacando.tipoDeAlvo == Alvos.Duro) { soundToPlay = SoundHolder.instancia.shellMiss; }
                else { soundToPlay = SoundHolder.instancia.bulletMiss; }
            }
            else if (unidadeAtacando.tipoDeAlvo == Alvos.Duro || unidadeAtacando.classe == Classe.AntiTank || unidadeAtacando.classe == Classe.Artilharia || unidadeAtacando.classe == Classe.RocketArtilharia
                  || unidadeAtacando.equipamento.Any(equip => equip.tipoDeEquipamento == EquipamentoTipo.AntiTanque)){
                soundToPlay = SoundHolder.instancia.artilleryHit;
            }
            else{
                if (unidadeAtacando.tipoDeAlvo == Alvos.Duro) { soundToPlay = SoundHolder.instancia.shellHit; }
                else { soundToPlay = SoundHolder.instancia.bulletsOnHardHit; }
            }
        }
        else{
            if (miss){ soundToPlay = SoundHolder.instancia.bulletMiss; }
            else     { soundToPlay = SoundHolder.instancia.bulletsHit; }
        }

        fonteDeSomDaUnidade.PlayOneShot(soundToPlay);
        return soundToPlay;
    }
    */
}
