using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Node : MonoBehaviour{
    public List<Node> vizinhos;//Para onde o player pode se mover a partir desse node

    public bool parteDoReich;//Se o node e parte veridica do Reich, dando alta taxa de suprimentos
    public bool armazem;//Se o node e um de reabastecimento, onde as unidades do player se recuperam
    public bool conquistado;//Se o node está livre da influencia de unidades inimigas
    public bool recentementeConquistada;//Se o node foi recentemente o palco de uma batalha, grandemente danificado sua infraestrutura
    public int countdownConquistado;//Quantos dias esse node precisa para não sofrer o efeito de ser recetemente conquistado
    public bool bossNode;//Se o node se trata do boss dessa campanha
    public int expBase;//Experiencia base que esse node dá ao ser terminado
    public int temperaturaBase;//Temperatura base do node, vai sofrer alterações de acordo com a estação do ano para virar a temperatura atual
    public int temperaturaAtual;//Temperatura do node, temperaturas extremas influenciam no atrito
    public bool pontoEstrategico;//E um dos pontos que pode levar a uma vitoria na campanha
    public Terrenos terreno;//Tipo de terreno do node, influencia nos tiles do mapa e no atrito
    public AudioClip musicaDeBackground;//Musica de background que tocará durante a batalha desse nó
    //public List<Unidade> dropList;//Lista de unidades que podem ser dropadas nesse node

    public TextAsset Fase1XMLPath;//Caminho para o XML contendo a primeira fase desse node
    public TextAsset Fase2XMLPath;//Idem só que para segunda
    public TextAsset Fase3XMLPath;//Idem só que para a terceira

    public Mundo0 nodeNoMundo0;//Nome desse node no mundo 0, meio feio mas vai ter que ser assim mesmo
    public Mundo1 nodeNoMundo1;
    
    public Pais nacionalidade;
    /*
    public Unidade sortearDrop(int battleRating){
        List<Unidade> unidadesPermitidas;
        int numeroAleatorio;
        switch (battleRating){
            case 3:
                unidadesPermitidas = dropList.Where(uni => uni.raridade <= 2).ToList();
                numeroAleatorio = UnityEngine.Random.Range(0, unidadesPermitidas.Count());
                return unidadesPermitidas[numeroAleatorio];
            case 4:
                unidadesPermitidas = dropList.Where(uni => uni.raridade <= 3).ToList();
                numeroAleatorio = UnityEngine.Random.Range(0, unidadesPermitidas.Count());
                return unidadesPermitidas[numeroAleatorio];
            case 5:
                numeroAleatorio = UnityEngine.Random.Range(0, dropList.Count());
                return dropList[numeroAleatorio];
            default:
                return null;
        }
    }*/
    
    public int calcularTemperatura(){
        System.Random rand = new System.Random();
        int aleatorio = rand.Next(-5,5);
        temperaturaAtual = temperaturaBase + aleatorio;//Influencia aleatoria de até 5 graus na temperatura
        switch (BancoDeDados.instancia.campanhasDoJogo[GameController.instancia.campanhaSelecionada].estacaoDaOfensiva){
            case Estacao.Primavera://Privamera aumenta a temperatura em até +5 graus
                temperaturaAtual += rand.Next(5);
                break;
            case Estacao.Verão://Verão aumenta grandemente a temperatura, entre 5 a 10 graus
                temperaturaAtual += rand.Next(5, 10);
                break;
            case Estacao.Outono://Outono diminui a temperatura em até -5 graus
                temperaturaAtual -= rand.Next(5);
                break;
            case Estacao.Inverno://Inverno diminui grandemente a temperatura, entre -5 a -10 graus
                temperaturaAtual -= rand.Next(5,10);
                break;
        }
        return temperaturaAtual;
    }

    public bool nodePacificado(){
        countdownConquistado--;
        if (countdownConquistado <= 0){
            recentementeConquistada = false;
            return true;
        }
        else{
            return false;
        }
    }

    public void nodeConquistado(){
        conquistado = true;
        recentementeConquistada = true;
        switch (terreno){
            //Esses terrenos não tem muito lugar em que uma resistencia pode se ploriferar e são pouco populosos
            case Terrenos.Deserto:
            case Terrenos.Pantâno:            
            case Terrenos.Congelado:
                countdownConquistado = 2;
                break;
            //Estes já podem ter uma população maior e mais lugares para se esconderem
            case Terrenos.Bosques:
            case Terrenos.Terroso:
            case Terrenos.Gelado:
                countdownConquistado = 4;
                break;
            //Colinas normalmente contam com grandes populações e terrenos montanhosos são perfeitos para resistencias se esconderem
            case Terrenos.Colinas:
            case Terrenos.Montanha:
                countdownConquistado = 6;
                break;
            //Populações de cidades densamentes povoadas vão resistir até o fim
            case Terrenos.Urbano:
                countdownConquistado = 10;
                break;
        }
    }
}

