using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LinhaDeProducao{//Classe que carrega informações da linha de produção
    public int slot;
    public float proguesso;//Proguesso atual, se chegar a 100 a producao termina
    public float ppm;//Proguesso por minuto, o quanto a barra de proguesso avança a cada minuto
    public ProducaoTipo tipoDeProducao = ProducaoTipo.Inativa;

    public bool ativa = false;//Se a linha de produção está ativamente produzindo algo
    public bool emEspera = false;//Se está sendo processado uma mudança de produção
    public bool terminada = false;//Se a linha terminou a producao atual

    public Unidade unidadeSendoProduzida;//Unidade que está sendo produzida por essa linha de produção
    public Nacoes nacaoSendoDesenvolvida;//Nação que está sendo atualmente desenvolvida por essa linha
    public Equipamento equipamentoSendoConstruido;//Equipamento sendo atualmente criado nessa linha de produção
    
    public IEnumerator produzir(){
        //No caso de desenvolvimento e recrutamento eles acabam quando chegam 100%        
        if (tipoDeProducao == ProducaoTipo.Desenvolvimento || tipoDeProducao == ProducaoTipo.Recrutamento || tipoDeProducao == ProducaoTipo.Construcao) {
            while (ativa && !emEspera && proguesso < 100){                    
                proguesso += ppm;
                if (proguesso >= 100) { proguesso=100; ativa = false; terminada = true; }
                if(LinhasDeProducaoController.instancia!=null){ LinhasDeProducaoController.instancia.atualizarVisual(this); }                                
                yield return new WaitForSeconds(0.1f);
            }
        }
        else{
            while (ativa && !emEspera){
                switch (tipoDeProducao) {
                    case ProducaoTipo.Metais: GameController.instancia.Metal += 3; break;
                    case ProducaoTipo.Combustivel: GameController.instancia.Combustivel += 3; break;
                    case ProducaoTipo.Municao: GameController.instancia.Municao += 3; break;
                    case ProducaoTipo.BensDeConsumo: GameController.instancia.MaoDeObra += 1; break;
                }
                if (EventosDoHQ.instancia != null) { EventosDoHQ.instancia.atualizarInterface(); }
                yield return new WaitForSeconds(0.1f);
            }      
        }
        yield break;
    }
}
