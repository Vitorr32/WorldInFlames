using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TilePathFinder : MonoBehaviour {
    //Aqui que vamos calcular o caminho caminho mais curto até o destino a partir da origem
	public static List<Tile> FindPathTilesReturn(Tile origem, Tile destino) {
		List<Tile> closed = new List<Tile>();
		List<TilePath> open = new List<TilePath>();

		UnidadeNoTabuleiro unidadeAtual = BatalhaController.instancia.unidadesNoTabuleiro[BatalhaController.instancia.unidadeAtualIndex];
		
		TilePath originPath = new TilePath();
		originPath.addTile(origem);
		
		open.Add(originPath);
		
		while (open.Count > 0) {
			open = open.OrderBy(x => x.custoDoCaminho).ToList();
			TilePath current = open[0];
			open.Remove(open[0]);
			
			if (closed.Contains(current.ultimoTile))continue;
			if (current.ultimoTile == destino) {
				current.listaDeTiles.Remove (origem);
				return current.listaDeTiles;
			}
			
			closed.Add(current.ultimoTile);
			
			foreach (Tile t in current.ultimoTile.vizinhos) {
				if (t.instransponivel) continue;
                if (unidadeAtual.gridDeMovimento && t.ocupado) continue;
				if (unidadeAtual.gridDeMovimento && t.classesRestritas.Contains (unidadeAtual.unidadeAssociada.classe))	continue;
				TilePath newTilePath = new TilePath(current);
				newTilePath.addTile(t);
				open.Add(newTilePath);
			}
		}
		return null;
	}

    public static TilePath FindPathTilePathReturn(Tile origem, Tile destino){
        List<Tile> closed = new List<Tile>();
        List<TilePath> open = new List<TilePath>();

        UnidadeNoTabuleiro unidadeAtual = BatalhaController.instancia.unidadesNoTabuleiro[BatalhaController.instancia.unidadeAtualIndex];

        TilePath originPath = new TilePath();
        originPath.addTile(origem);

        open.Add(originPath);

        while (open.Count > 0){
            open = open.OrderBy(x => x.custoDoCaminho).ToList();
            TilePath current = open[0];
            open.Remove(open[0]);

            if (closed.Contains(current.ultimoTile)) continue;
            if (current.ultimoTile == destino){
                current.listaDeTiles.Remove(origem);
                return current;
            }

            closed.Add(current.ultimoTile);

            foreach (Tile t in current.ultimoTile.vizinhos){
                if (t.instransponivel) continue;
                if (unidadeAtual.gridDeMovimento && t.ocupado) continue;
                if (unidadeAtual.gridDeMovimento && t.classesRestritas.Contains(unidadeAtual.unidadeAssociada.classe)) continue;
                TilePath newTilePath = new TilePath(current);
                newTilePath.addTile(t);
                open.Add(newTilePath);
            }
        }
        return null;
    }
}

