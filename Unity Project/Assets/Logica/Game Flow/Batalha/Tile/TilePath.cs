using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TilePath {
	public List<Tile> listaDeTiles = new List<Tile>();

	public int custoDoCaminho = 0;	//Serve para saber até onde esse caminho vai, ao recuperar esse valor o
    //TileHighlight vai saber se deve continuar a processar esse caminho ou guardar ele no "closed"
	public Tile ultimoTile;//Auto-explicatorio,
	
	public TilePath() {
    }
	
	public TilePath(TilePath tp) {
		listaDeTiles = tp.listaDeTiles.ToList();
		custoDoCaminho = tp.custoDoCaminho;
		ultimoTile = tp.ultimoTile;
	}
	
	public void addTile(Tile t) {//Adicionar a lista simples, mudando quem é o ultimo tile no processo
		if(BatalhaController.instancia.unidadesNoTabuleiro[BatalhaController.instancia.unidadeAtualIndex].gridDeMovimento)custoDoCaminho += t.custoDeMovimento;
        else custoDoCaminho+=1;
		listaDeTiles.Add(t);
		ultimoTile = t;
	}
}