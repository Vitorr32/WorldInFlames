using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapCreatorManager : MonoBehaviour {
	public static MapCreatorManager instance;

    public GameObject mapHolder;
 	public int colunas;
    public int linhas;
	public List <List<Tile>> tabuleiro = new List<List<Tile>>();
    public List<UnidadeNoTabuleiro> unidadesNoTabuleiro= new List<UnidadeNoTabuleiro>();
    public GameObject tilePrefab;
    public GameObject unidadePrefab;

    public RuntimeAnimatorController[] controladoresInimigos;

    public TileType palletSelection = TileType.Null;
    public TileMechanic mechanicPallet = TileMechanic.NO_MECHANIC;

    public int idDaUnidadeSelecionada ;
    public bool mudandoMecanica = false;
    public bool mudandoTipo = false;
    public bool colocandoUnidade = false;

    public UnidadeNoTabuleiro unidadeSendoSetada;
    public BancoDeDados banco;
    void Awake () {
		instance = this;
	}

    void Update(){
        if(Input.GetMouseButtonDown(1) && colocandoUnidade){
            colocandoUnidade = false;
            Destroy(unidadeSendoSetada.gameObject);
            unidadeSendoSetada = null;
        }
    }

    public void destruirTabuleiroAtual(){
        if (mapHolder.transform.childCount != 0){
            var children = new List<GameObject>();
            foreach (Transform child in mapHolder.transform) children.Add(child.gameObject);
            foreach (UnidadeNoTabuleiro child in unidadesNoTabuleiro) children.Add(child.gameObject);
            children.ForEach(child => Destroy(child));
        }
        tabuleiro = new List<List<Tile>>();
        unidadesNoTabuleiro = new List<UnidadeNoTabuleiro>();
    }

	public void generateBlankMap(int colunasTabuleiro,int linhasTabuleiro) {
		colunas = colunasTabuleiro;
        linhas = linhasTabuleiro;

        destruirTabuleiroAtual();
        
        for(int i =0;i< colunasTabuleiro; i++){
            List<Tile> Row = new List<Tile>();
            for(int j = 0; j < linhasTabuleiro; j++){
                Tile tileNovo = ((GameObject)Instantiate(tilePrefab, new Vector3(i - Mathf.Floor(linhasTabuleiro/2),0, -j - Mathf.Floor(colunasTabuleiro / 2)), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();
                tileNovo.setarTipoeVisual(TileType.Null);
                tileNovo.posicaoNoTabuleiro = new Vector2(i, j);
                tileNovo.transform.SetParent(mapHolder.transform);
                Row.Add(tileNovo);
            }
            tabuleiro.Add(Row);
        }
        configurarCameraParaNovoTabuleiro(tabuleiro);
	}

	public void loadMapFromXml(string nome) {
		MapXmlContainer container = MapSaveLoad.MapBuildLoad(nome+".xml");

		colunas = container.coluns;
        linhas = container.lines;

        destruirTabuleiroAtual();

		for (int i = 0; i < colunas; i++) {
			List <Tile> row = new List<Tile>();
			for (int j = 0; j < linhas; j++) {
				Tile tile = ((GameObject)Instantiate(tilePrefab, new Vector3(i - Mathf.Floor(linhas/2),0, -j + Mathf.Floor(colunas/2)), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();
				tile.transform.parent = mapHolder.transform;
                tile.noConstrutor = true;
				tile.posicaoNoTabuleiro = new Vector2(i, j);
				tile.setarTipoeVisual((TileType)container.tiles.Where(x => x.locX == i && x.locY == j).First().id);
                tile.setarMecanicaeVisual((TileMechanic)container.tiles.Where(x => x.locX == i && x.locY == j).First().mechanic);
                row.Add (tile);
			}
			tabuleiro.Add(row);
		}
        foreach(UnidadeXml uXML in container.unidadesNoTabuleiro){
            UnidadeNoTabuleiro unidadeNova = ((GameObject)Instantiate(unidadePrefab,tabuleiro[uXML.locX][uXML.locY].transform.position + 1 * Vector3.up, Quaternion.Euler(new Vector3(90,0,0)))).GetComponent<UnidadeNoTabuleiro>();
            unidadeNova.gridPosition = new Vector2(uXML.locX,uXML.locY);
            unidadeNova.unidadeAssociada = BancoDeDados.instancia.retornarUnidade(uXML.id);
            unidadeNova.gameObject.GetComponent<Animator>().runtimeAnimatorController = controladoresInimigos[(int)unidadeNova.unidadeAssociada.classe];
            //Aqui so entrarão unidades que vão estar contra você no jogo, então sempre serão inimigos
            unidadeNova.inimigo = true;
            tabuleiro[uXML.locX][uXML.locY].ocupado = true;
            unidadesNoTabuleiro.Add(unidadeNova);
        }
        configurarCameraParaNovoTabuleiro(tabuleiro);
    }

    public void instanciarUnidade(int id){
        if(unidadeSendoSetada!=null) {unidadeSendoSetada = null; }
        UnidadeNoTabuleiro unidadeNova = ((GameObject)Instantiate(unidadePrefab)).GetComponent<UnidadeNoTabuleiro>();
        unidadeSendoSetada = unidadeNova;
        unidadeSendoSetada.unidadeAssociada = banco.retornarUnidade(id);
        unidadeSendoSetada.gameObject.GetComponent<Animator>().runtimeAnimatorController = controladoresInimigos[(int)unidadeNova.unidadeAssociada.classe];
    }

    public void reposicionarUnidade(Tile tile){
        tile.ocupado = false;        
        if (unidadeSendoSetada != null) { unidadeSendoSetada = null; }
        unidadeSendoSetada = unidadesNoTabuleiro.First(uni => uni.gridPosition == tile.posicaoNoTabuleiro);
        unidadesNoTabuleiro.Remove(unidadeSendoSetada);
    }

    public void setarUnidadeNoSlot(Tile tile){
        unidadeSendoSetada.gridPosition = tile.posicaoNoTabuleiro;
        unidadesNoTabuleiro.Add(unidadeSendoSetada);
        colocandoUnidade = false;
        tile.ocupado = true;
        unidadeSendoSetada = null;
        //instanciarUnidade(unidadeSendoSetada.unidadeAssociada.id);
    }

	public void saveMapToXml(string nome) {
		MapSaveLoad.Save(MapSaveLoad.CreateMapContainer(tabuleiro,unidadesNoTabuleiro,linhas,colunas), nome+".xml");
	}

    public void configurarCameraParaNovoTabuleiro(List<List<Tile>> tabuleiroASerConfigurado){
        CameraController.instancia.setarTabuleiro(tabuleiroASerConfigurado);
        CameraController.instancia.calcularLimitesDaCamera();
        CameraController.instancia.olharProCentroDoTabuleiro();
    }
}

