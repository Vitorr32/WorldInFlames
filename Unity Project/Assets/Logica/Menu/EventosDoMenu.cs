using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventosDoMenu : MonoBehaviour {
    public Button mapBuilder;
    public Node no;

    void Start(){
        if (Application.isEditor) mapBuilder.gameObject.SetActive(false);
    }
    
    public void NovoJogo(){
        GameController.instancia.setarCampanhaAtiva(0);
        GameController.instancia.ativarCampanhas();
        //DEBUG
        GameController.instancia.AdicionarUnidadeAoJogador(BancoDeDados.instancia.retornarUnidade(2));
        GameController.instancia.AdicionarUnidadeAoJogador(BancoDeDados.instancia.retornarUnidade(3));

        LinhaDeProducao linhaTeste = new LinhaDeProducao();
        GameController.instancia.linhasDeProducao.Add(linhaTeste);
        LinhaDeProducao linhaTeste2 = new LinhaDeProducao();
        GameController.instancia.linhasDeProducao.Add(linhaTeste2);

        Nacoes Alemanha = BancoDeDados.instancia.retornarNacao(Pais.Alemanha);
        GameController.instancia.nacoesDoJogador.Add(Alemanha);

        //GameController.instancia.primeiroBatalhao[0].expUp(40000);

        GameController.instancia.organizarEquipamentoPorTipo();
        GameController.instancia.organizarUnidadesPorTipo();

        GameController.instancia.Combustivel += 50000;
        GameController.instancia.Metal += 50000;
        GameController.instancia.Municao += 100000;
        GameController.instancia.MaoDeObra += 10000;
        GameController.instancia.Inspiracao += 1000;
        //DEBUG
        SceneManager.LoadScene("HQ");
    }

    public void mapaBuilder(){
        SceneManager.LoadScene("CriacaoDeBattlegrounds");
    }

    public void CarregarJogo(){
        GameController.SaveLoad.Load();

        GameController.instancia.organizarEquipamentoPorTipo();
        GameController.instancia.organizarUnidadesPorTipo();

        SceneManager.LoadScene("HQ");
    }

    public void Creditos() {
        //DEBUGGING
        GameController.instancia.primeiroBatalhao.Add(BancoDeDados.instancia.retornarUnidade(2));
        GameController.instancia.primeiroBatalhao.Add(BancoDeDados.instancia.retornarUnidade(3));
        
        GameController.instancia.rushFront = false;

        GameController.instancia.nodeAtual = no;
        SceneManager.LoadScene("BattleScene");

    }
}
