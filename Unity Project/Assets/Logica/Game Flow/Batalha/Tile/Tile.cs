using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Logica.Classes;
using UnityEngine.SceneManagement;

public class Tile : MonoBehaviour {
    public Vector2 posicaoNoTabuleiro = Vector2.zero;
    public TileType tipoDeTile = TileType.Null;
    public TileMechanic tipoDeMecanica = TileMechanic.NO_MECHANIC;
    public List<Tile> vizinhos = new List<Tile>();
    public List<Classe> classesRestritas = new List<Classe>();//Quais classes não podem acessar esse tile
    //Tipos de Tile especiais
    public bool pontoDeInicio = false;//Se esse tile e um dos pontos de inicio da fase
    public bool reforcoDoFront = false;
    public bool ataqueDoSuporte = false;
    //Tiles especiais vão influenciar na decisão dos jogadores no gameplay
    public Material materialOriginalDoTile;
    public bool instransponivel= false;//Tiles instransponiveis não podem ser usados por ninguem
    public bool ocupado = false;//Tiles ocupados atualmente tem alguem neles
    public int custoDeMovimento = 1;
    public bool sobreHighlight=false;
    //Para administrar tile fora ou dentro do construtor de mapas
    public bool noConstrutor = false;
    //Efeitos do tile sobre a batalha
    public List<int[]> efeitos = new List<int[]>();

	void Start () {
        if (!SceneManager.GetActiveScene().name.Equals("CriacaoDeBattlegrounds")) { GerarVizinhos();  noConstrutor = false; }
        else { noConstrutor = true; }
        if (tipoDeTile == TileType.Null) { setarTipoeVisual(TileType.Plano);}
	}
	
    void GerarVizinhos(){ //Vizinhos são os tiles, acima, abaxio, a esqueda e direita deste tile, se houver
        vizinhos = new List<Tile>();		
        //Se não tiver na parte de baixo do tabuleiro
		if (posicaoNoTabuleiro.y > 0) {
			Vector2 n = new Vector2(posicaoNoTabuleiro.x, posicaoNoTabuleiro.y - 1);
			vizinhos.Add(BatalhaController.instancia.tabuleiro[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
		}
		//Se não tiver na parte de cima do tabuleiro
		if (posicaoNoTabuleiro.y < BatalhaController.instancia.linhasTabuleiro - 1) {
			Vector2 n = new Vector2(posicaoNoTabuleiro.x, posicaoNoTabuleiro.y + 1);
			vizinhos.Add(BatalhaController.instancia.tabuleiro[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
		}		
		
		//Se estiver não estiver no canto esquerdo do tabuleiro
		if (posicaoNoTabuleiro.x > 0){
			Vector2 n = new Vector2(posicaoNoTabuleiro.x - 1, posicaoNoTabuleiro.y);
			vizinhos.Add(BatalhaController.instancia.tabuleiro[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
		}
		//Se não estiver no canto direito do tabuleiro
		if (posicaoNoTabuleiro.x < BatalhaController.instancia.colunasTabuleiro - 1) {
			Vector2 n = new Vector2(posicaoNoTabuleiro.x + 1, posicaoNoTabuleiro.y);
			vizinhos.Add(BatalhaController.instancia.tabuleiro[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
		}
    }

    public void mudarMaterial(Material novoMaterial){
        this.GetComponent<MeshRenderer>().material = novoMaterial;
        sobreHighlight = true;
    }

    public void voltarProOriginal(){
        GetComponent<MeshRenderer>().material = materialOriginalDoTile;
        sobreHighlight = false;
    }
    
    void OnMouseDown(){
        if (Input.GetMouseButtonDown(0)){
            if (!noConstrutor){
                if (BatalhaController.instancia.preBattle && !ocupado && sobreHighlight){
                    BatalhaController.instancia.setarUnidadeNoSlot(this);
                }
                else if (BatalhaController.instancia.preBattle && ocupado){
                    BatalhaController.instancia.reposicionarUnidadeNoTabuleiro(this);
                }
                else{
                    UnidadeNoTabuleiro atual = BatalhaController.instancia.unidadesNoTabuleiro[BatalhaController.instancia.unidadeAtualIndex];
                    if (atual.gridDeMovimento){
                        BatalhaController.instancia.verificarTentativaDeMovimento(this);
                    }
                    else if (atual.gridDeAtaque){
                        BatalhaController.instancia.atacarComPlayerAtual(this);
                    }
                    else if (!atual.gridDeAtaque && !atual.gridDeMovimento && ocupado) {
                        BatalhaController.instancia.mostrarInformacoesDaUnidadeClicada(this);
                    }
                }
            }
            else{
                if (MapCreatorManager.instance.mudandoTipo){
                    setarTipoeVisual(MapCreatorManager.instance.palletSelection);
                }
                else if (MapCreatorManager.instance.mudandoMecanica){
                    setarMecanicaeVisual(MapCreatorManager.instance.mechanicPallet);
                }
                else if (MapCreatorManager.instance.colocandoUnidade && !ocupado){
                    MapCreatorManager.instance.setarUnidadeNoSlot(this);
                }
                else if (!MapCreatorManager.instance.colocandoUnidade && ocupado){
                    MapCreatorManager.instance.colocandoUnidade = true;
                    MapCreatorManager.instance.reposicionarUnidade(this);
                }
            }
        }
    }

    void OnMouseEnter(){
		if (!noConstrutor) {
			if (BatalhaController.instancia != null && BatalhaController.instancia.preBattle) {
                if(sobreHighlight){
                    BatalhaController.instancia.unidadeSendoSetada.transform.position = this.transform.position + Vector3.up;
                }
            }
		}
        else if(MapCreatorManager.instance.colocandoUnidade){
            MapCreatorManager.instance.unidadeSendoSetada.transform.position = this.transform.position + Vector3.up;
        }
    }
    

    public void setarMecanicaeVisual(TileMechanic t){
        pontoDeInicio = false;
        ataqueDoSuporte = false;
        reforcoDoFront = false;
        tipoDeMecanica = t;
        switch (t){
            case TileMechanic.NO_MECHANIC:
                materialOriginalDoTile = CarregadorDePrefabs.instance.NO_MECHANIC;
                break;
            case TileMechanic.PLAYER_RETREAT_LINE:
                materialOriginalDoTile = CarregadorDePrefabs.instance.PLAYER_RETREAT_LINE;
                pontoDeInicio = true;
                break;
            case TileMechanic.PLAYER_SPAWN_MAIN:
                if (noConstrutor) { materialOriginalDoTile = CarregadorDePrefabs.instance.PLAYER_SPAWN_MAIN; }
                else { materialOriginalDoTile = CarregadorDePrefabs.instance.NO_MECHANIC; }
                pontoDeInicio = true;
                break;
			case TileMechanic.PLAYER_SPAWN_REINFORCEMENT:
                if (noConstrutor) { materialOriginalDoTile = CarregadorDePrefabs.instance.PLAYER_SPAWN_REINFORCEMENT; }
                else { materialOriginalDoTile = CarregadorDePrefabs.instance.NO_MECHANIC; }
                pontoDeInicio = true;
                break;
            case TileMechanic.ENEMY_RETREAT_LINE:
                if (noConstrutor) { materialOriginalDoTile = CarregadorDePrefabs.instance.ENEMY_RETREAT_LINE; }
                else { materialOriginalDoTile = CarregadorDePrefabs.instance.NO_MECHANIC; }
                pontoDeInicio = true;
                break;
            case TileMechanic.ENEMY_SPAWN_REINFORCEMENT:
                if (noConstrutor) { materialOriginalDoTile = CarregadorDePrefabs.instance.ENEMY_SPAWN_REINFORCEMENT; }
                else { materialOriginalDoTile = CarregadorDePrefabs.instance.NO_MECHANIC; }
                pontoDeInicio = true;
                break;
            case TileMechanic.ENEMY_SPAWN_SUPPORT:
                if (noConstrutor) { materialOriginalDoTile = CarregadorDePrefabs.instance.ENEMY_SPAWN_SUPPORT; }
                else { materialOriginalDoTile = CarregadorDePrefabs.instance.NO_MECHANIC; }
                pontoDeInicio = true;
                break;
            case TileMechanic.PLAYER_ATTACK_SUPPORT:
                materialOriginalDoTile = CarregadorDePrefabs.instance.PLAYER_ATTACK_SUPPORT;
                ataqueDoSuporte = true;
                break;
        }
        GetComponent<Renderer>().material = materialOriginalDoTile;
    }

    public void setarTipoeVisual(TileType t) {
        /*Efeitos são arquivados numa lista que cada elemente contém 2 numeros, da seguinte forma:
                        O primeiro representando qual dos status da unidade sofrerá um buff/debuff
                        O segundo representando o quando, em porcentagem esse estado vai ser modificado(Pode ser negativo)

            Efeitos Trigerados são diferentes, esses apesar se parecidos com o efeito comum tem um trigger que é se estão
            defendendo ou atacando e contra certo tipo de tile, por exemplo se uma unidade no tile colina está atacando
            uma unidade numa planice, a unidade na colina vai receber um bonus de precisão por estar "acima" do inimigo
            dando um boost de precisão, seria da seguinte forma

                O primeiro representando qual dos status da unidade sofrerá um buff/debuff
                O segundo representando o quando, em porcentagem esse estado vai ser modificado(Pode ser negativo)
                O terceiro e um 0 ou 1, zero representando defesa e o 1 ataque, qual desses triggera o efeito
                O quarto ou mais são os tipo de terrenos a partir do Enum que esse efeito e triggerado, pode ser varios portanto pode ser mais de 1
                

            Especial : Unidade podem ser imunes ao efeito do tiles se forem de certa classe,
            Esse efeito e representando da seguinte forma: o primeiro numero é -1, para representar que não se trata de um status
            O segundo numero e o numero que representa o Enum daquela classe da unidade


            
         */
        tipoDeTile = t;
        GameObject newVisual;
		//definitions of TileType properties
		switch(t) {
			case TileType.Plano:
                newVisual = (GameObject)Instantiate(CarregadorDePrefabs.instance.PLANICE, transform.position, Quaternion.Euler(new Vector3(0,0,0)));
                instransponivel = false;

                efeitos.Add(new int[] { 9, -20});//Campo Aberto, sem nenhuma cobertura -20% de Esquiva
                efeitos.Add(new int[] { 4,  25}); //Campo aberto, com ampla visão +25% de Alcance

                custoDeMovimento = 2;
                break;			
			case TileType.Colina:
                newVisual = (GameObject)Instantiate(CarregadorDePrefabs.instance.COLINA, transform.position, Quaternion.Euler(new Vector3(0,0,0)));
				instransponivel= false;

                efeitos.Add(new int[] { 9, -10 });//Campo Semi-Aberto, com pouca cobertura -10% de Esquiva
                efeitos.Add(new int[] { 4, 15 }); //Campo Semi-Aberto, com boa visão +15% de Alcance
                efeitos.Add(new int[] { 8, 10 }); //Campo Semi-Alto, com boa visão do inimigo +10% de Precisão

                custoDeMovimento = 3;
                break;				
			case TileType.Deserto:
                newVisual = (GameObject)Instantiate(CarregadorDePrefabs.instance.DESERTO, transform.position, Quaternion.Euler(new Vector3(0,0,0)));
				instransponivel= false;

                efeitos.Add(new int[] { 9, -20 });//Campo Aberto, com pouca cobertura -20% de Esquiva
                efeitos.Add(new int[] { 4, 25 }); //Campo Aberto, com boa visão +25% de Alcance

                custoDeMovimento = 4;
                classesRestritas.Add(Classe.TanqueUltraPesado);
                classesRestritas.Add(Classe.TanquePesado);
                break;

            case TileType.Rio:
                newVisual = (GameObject)Instantiate(CarregadorDePrefabs.instance.RIO, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                instransponivel = false;

                efeitos.Add(new int[] { 9, -50 });//Campo Inundando, movimentação quase impossivel -50% de Esquiva
                efeitos.Add(new int[] { 8, -25 });//Campo Inundando, com apoio horrivel -25% de Alcance
                efeitos.Add(new int[] { -1, (int)Classe.InfantariaNaval });//Imunidade de campo inundando para Marines

                custoDeMovimento = 5;
                break;
            case TileType.GrandeRio:
                newVisual = (GameObject)Instantiate(CarregadorDePrefabs.instance.GRANDERIO, transform.position, Quaternion.Euler(new Vector3(0,0,0)));
				instransponivel= false;

                efeitos.Add(new int[] { 9, -50 });//Campo Inundando, movimentação quase impossivel -50% de Esquiva
                efeitos.Add(new int[] { 8, -25 });//Campo Inundando, com apoio horrivel -25% de Alcance
                efeitos.Add(new int[] { -1, (int)Classe.InfantariaNaval });//Imunidade de campo inundando para Marines

                custoDeMovimento = 5;
                break;
            case TileType.NeveLeve:
                newVisual = (GameObject)Instantiate(CarregadorDePrefabs.instance.NEVELEVE, transform.position, Quaternion.Euler(new Vector3(0,0,0)));
				instransponivel= false;

                efeitos.Add(new int[] { 9, -10 });//Campo Dificil, movimentação dificil -10% de Esquiva
                efeitos.Add(new int[] { 8, -15 });//Campo Dificil, com apoio ruim -15% de Alcance
                efeitos.Add(new int[] { -1, (int)Classe.InfantariaAlpina });//Imunidade de campo nevoso para Infantaria Alpina

                custoDeMovimento = 3;
                break;
            case TileType.NeveFunda:
                newVisual = (GameObject)Instantiate(CarregadorDePrefabs.instance.NEVEFUNDA, transform.position, Quaternion.Euler(new Vector3(0,0,0)));
				instransponivel= false;

                efeitos.Add(new int[] { 9, -20 });//Campo Muito Dificil, movimentação dificil -20% de Esquiva
                efeitos.Add(new int[] { 8, -25 });//Campo Muito Dificil, com apoio ruim -25% de Alcance
                efeitos.Add(new int[] { -1, (int)Classe.InfantariaAlpina });//Imunidade de campo nevoso para Infantaria Alpina

                custoDeMovimento = 4;
                break;
            case TileType.Ponte:
                newVisual = (GameObject)Instantiate(CarregadorDePrefabs.instance.PONTE, transform.position, Quaternion.Euler(new Vector3(0,0,0)));
				instransponivel= false;

                efeitos.Add(new int[] { 9, -40 });//Campo Estreito e Sem cobertura, movimentação dificil -40% de Esquiva

                custoDeMovimento = 2;
                break;
            case TileType.Rua:
                newVisual = (GameObject)Instantiate(CarregadorDePrefabs.instance.RUA, transform.position, Quaternion.Euler(new Vector3(0,0,0)));
				instransponivel= false;

                efeitos.Add(new int[] { 4, 20 });//Campo Aberto, com pouca cobertura +20% de Alcance
                efeitos.Add(new int[] { 9, 20 });//Campo com boa Infraestrutura, movimentação fluida +20% de Esquiva
                efeitos.Add(new int[] { 8, 25 });//Campo com boa Infraestrutura, otimo apoio +25% de Precisão

                custoDeMovimento = 1;
                break;
            case TileType.Terra:
                newVisual = (GameObject)Instantiate(CarregadorDePrefabs.instance.TERRA, transform.position, Quaternion.Euler(new Vector3(0,0,0)));
				instransponivel= false;

                efeitos.Add(new int[] { 9, -10 });//Campo Aberto, com algumas coberturas -10% de Esquiva
                efeitos.Add(new int[] { 4, 10 }); //Campo aberto, com boa visão +10% de Alcance

                custoDeMovimento = 3;
                break;
            case TileType.Montanha:
                newVisual = (GameObject)Instantiate(CarregadorDePrefabs.instance.MONTANHA, transform.position, Quaternion.Euler(new Vector3(0,0,0)));
				instransponivel= false;

                efeitos.Add(new int[] { 9, 25 });//Campo Montanhoso, grandes areas de cobertura e dificil acesso +25% de Esquiva
                efeitos.Add(new int[] { 4, 15 }); //Campo Montanhoso, com boa visão +15% de Alcance
                efeitos.Add(new int[] { 8, 20 }); //Campo Alto, com boa visão do inimigo +20% de Precisão
                efeitos.Add(new int[] { 10, 20 }); //Campo Montanhoso, Proteção a paritr do terreno 20% de Rigidez

                custoDeMovimento = 6;
                break;
            case TileType.Floresta:
                newVisual = (GameObject)Instantiate(CarregadorDePrefabs.instance.FLORESTA, transform.position, Quaternion.Euler(new Vector3(0,0,0)));
				instransponivel= false;

                efeitos.Add(new int[] { 9, 15 });//Campo Florestal, Cobertura vegetal e dificil visão para o inimigo +15% de Esquiva
                efeitos.Add(new int[] { 4, -15 }); //Campo Florestal, Cobertura vegetal e baixa visão do inimigo -15% de Alcance

                custoDeMovimento = 4;
                break;
            case TileType.Pantano:
                newVisual = (GameObject)Instantiate(CarregadorDePrefabs.instance.PANTANO, transform.position, Quaternion.Euler(new Vector3(0,0,0)));
		        instransponivel = false;

                efeitos.Add(new int[] { 9, -40 });//Campo Inundando e dificil, movimentação quase impossivel -40% de Esquiva
                efeitos.Add(new int[] { 8, -45 });//Campo Inundando e dificil, com apoio horrivel -40% de Alcance
                efeitos.Add(new int[] { -1, (int)Classe.InfantariaNaval });//Imunidade de campo inundando para Marines

                custoDeMovimento = 6;
                classesRestritas.Add(Classe.TanqueLeve);
                classesRestritas.Add(Classe.TanqueMedio);
                classesRestritas.Add(Classe.TanqueUltraPesado);
                classesRestritas.Add(Classe.TanquePesado);
                break;
            case TileType.Urbano:
                newVisual = (GameObject)Instantiate(CarregadorDePrefabs.instance.URBANO, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                instransponivel = false;

                efeitos.Add(new int[] { 9, 30 });//Campo Urbano, Grandes areas urbanas e coberturas +30% de Esquiva
                efeitos.Add(new int[] { 4, -30 }); //Campo Urbano, Dificil visão do inimigo -30% de Alcance
                efeitos.Add(new int[] { 10, 40 }); //Campo Urbano, Proteção das construções 40% de Rigidez

                custoDeMovimento = 3;
                break;
            default:
                newVisual = (GameObject)Instantiate(CarregadorDePrefabs.instance.NULA, transform.position, Quaternion.Euler(new Vector3(0,0,0)));
                instransponivel = false;
                custoDeMovimento = 0;
            break;
		}        
        if(transform.childCount!=0)Destroy(transform.GetChild(0).gameObject);
        newVisual.transform.parent = transform;
	}

}


