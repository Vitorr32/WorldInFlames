using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using System.Text.RegularExpressions;

public class TileXml {
	[XmlAttribute("id")]
	public int id;//Qual tipo de tile é, de acordo com o Enum do TileType
    [XmlAttribute("mechanic")]
    public int mechanic;//Qual tipo de tile é, de acordo com o Enum do TileType
    [XmlAttribute("locX")]
	public int locX;//Posição no grid , valor X
	[XmlAttribute("locY")]
	public int locY;//Posição no grid, valor Y
}

public class UnidadeXml{
    [XmlAttribute("id")]
	public int id;//Qual o id da unidade inimiga, pra ser recuperada no Banco de Dados
	[XmlAttribute("locX")]
	public int locX;//Posição no grid , valor X
	[XmlAttribute("locY")]
	public int locY;//Posição no grid, valor Y
}

[XmlRoot("MapCollection")]
public class MapXmlContainer {
    [XmlAttribute("lines")]
	public int lines;
    [XmlAttribute("coluns")]
	public int coluns;

	[XmlArray("Tiles")]
	[XmlArrayItem("Tile")]
	public List<TileXml> tiles = new List<TileXml>();
    [XmlArray("Units")]
	[XmlArrayItem("Unit")]
	public List<UnidadeXml> unidadesNoTabuleiro = new List<UnidadeXml>();
}

public static class MapSaveLoad {
    
	public static MapXmlContainer CreateMapContainer(List <List<Tile>> map, List<UnidadeNoTabuleiro> unidades, int linhas, int colunas) {

		List<TileXml> tiles = new List<TileXml>();
        List<UnidadeXml> unidadesXML = new List<UnidadeXml>();
		for(int i = 0; i < colunas; i++) {
			for (int j = 0; j < linhas; j++) {
				tiles.Add(MapSaveLoad.CreateTileXml(map[i][j]));
			}
		}
        foreach(UnidadeNoTabuleiro u in unidades){
            unidadesXML.Add(MapSaveLoad.CreateUnidadeXml(u));
        }
        

		return new MapXmlContainer() {
			lines = linhas,
            coluns = colunas,
            unidadesNoTabuleiro = unidadesXML,
			tiles = tiles
		};
	}
    
	public static TileXml CreateTileXml(Tile tile) {
        return new TileXml() {
            id = (int)tile.tipoDeTile,
            mechanic = (int)tile.tipoDeMecanica,
			locX = (int)tile.posicaoNoTabuleiro.x,
			locY = (int)tile.posicaoNoTabuleiro.y
		};
	}

    public static UnidadeXml CreateUnidadeXml(UnidadeNoTabuleiro unidade) {
        return new UnidadeXml() {
            id = (int)unidade.unidadeAssociada.idNoBancoDeDados,
			locX = (int)unidade.gridPosition.x,
			locY = (int)unidade.gridPosition.y
		};
	}

    
	public static void Save(MapXmlContainer mapContainer, string filename) {
		var serializer = new XmlSerializer(typeof(MapXmlContainer));
        string qualMundo = Regex.Match(filename, @"\d+").Value;
        int world = Int32.Parse(qualMundo);
        filename = Application.dataPath + "/Resources/XML/Mapas/World"+world+"/" + filename;
        using (var stream = new FileStream(filename, FileMode.Create)){
			serializer.Serialize(stream, mapContainer);
		}
	}

	public static MapXmlContainer MapBuildLoad(string filename) {
        string caminho = Application.dataPath;
        string qualMundo = Regex.Match(filename, @"\d+").Value;
        int world = Int32.Parse(qualMundo);
        caminho = Application.dataPath+"/Resources/XML/Mapas/World"+world+"/"+filename;
        var serializer = new XmlSerializer(typeof(MapXmlContainer));
		using(var stream = new FileStream(caminho, FileMode.Open)){
			return serializer.Deserialize(stream) as MapXmlContainer;
		}
	}

    public static MapXmlContainer OnGameLoad(int fase){
        var serializer = new XmlSerializer(typeof(MapXmlContainer));
        TextAsset textAsset = null;

        switch (fase){
            case 1: textAsset = GameController.instancia.nodeAtual.Fase1XMLPath; break;
            case 2: textAsset = GameController.instancia.nodeAtual.Fase2XMLPath; break;
            case 3: textAsset = GameController.instancia.nodeAtual.Fase3XMLPath; break;
        }

        using(var reader = new System.IO.StringReader(textAsset.text)){
            return serializer.Deserialize(reader) as MapXmlContainer;
        }
    }
}
