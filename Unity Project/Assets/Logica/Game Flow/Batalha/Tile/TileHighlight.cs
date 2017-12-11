using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TileHighlight {
	
	public TileHighlight () {
		
	}
	//Esse highlight vai "atirar" pra todo lado, sem um destino especifico, apenas indo até os pontosDeMovimento deixarem
	public static List<Tile> FindHighlight(Tile tileDeOrigem, int pontosDeMovimento) {
		List<Tile> closed = new List<Tile>();//aqui são aqueles Tiles que ja foram "computados" e que vão voltar pro BatalhaController
		List<TilePath> open = new List<TilePath>();//Tiles que ainda não foram processados
		UnidadeNoTabuleiro unidadeAtual = BatalhaController.instancia.unidadesNoTabuleiro[BatalhaController.instancia.unidadeAtualIndex];
		TilePath caminhoDeOrigem = new TilePath();//Começando um um novo caminho
		caminhoDeOrigem.addTile(tileDeOrigem);//Esse caminho parte da origem
		
		open.Add(caminhoDeOrigem);//Guardando esse caminho nas tiles opens, porque se tu der closed na origem so vai da pra fazer o primeiro caminho
		
		while (open.Count > 0) {//Enquanto houver tiles ainda não processados....
			TilePath atual = open[0];
			open.Remove(open[0]);
			if (closed.Contains(atual.ultimoTile))continue;//Se esse tile ja foi fechado, não há porque computar	            
			if (atual.custoDoCaminho > pontosDeMovimento + 1)continue;//Se acabou os pontos de movimento	
            closed.Add(atual.ultimoTile);
			foreach (Tile t in atual.ultimoTile.vizinhos) {	
				if (t.instransponivel) continue;
				if ((t.ocupado && !unidadeAtual.gridDeAtaque)) continue;
				if (unidadeAtual.gridDeMovimento && t.classesRestritas.Contains (unidadeAtual.unidadeAssociada.classe))	continue;
				TilePath newTilePath = new TilePath(atual);
				newTilePath.addTile(t);
				open.Add(newTilePath);
			}
		}
        //Salvador, aqui vai verificar se caso os pontos de movimento forem muito poucos, adicionar todos os vizinhos
        //da origem pra evitar atolamento, isso não ignora tiles instraponiveis ou ocupados, apenas o custo da caminho
        if (unidadeAtual.gridDeMovimento){
            //Checar se tem algo na lista de tiles além dos vizinhos do tile de origem
            //Caso tenha e porque ele pode ser mover em alguma direção além dessa sem estar atolado
            if (closed.Count == 0 || closed.Any(x => tileDeOrigem.vizinhos.Any(y => x != y))){
                foreach(Tile vizinho in tileDeOrigem.vizinhos){
                    if(vizinho.instransponivel || vizinho.ocupado || 
                       vizinho.classesRestritas.Contains(unidadeAtual.unidadeAssociada.classe)) { continue; }
                    closed.Add(vizinho);
                }
            }
        }


		closed.Remove(tileDeOrigem);
		return closed;
	}
}
