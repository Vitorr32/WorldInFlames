using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Logica.Classes;

public class BatalhaTileInfo : MonoBehaviour {
    public Image imagemDoTerreno;
    public Text nomeDoTerreno;

    public GameObject[] efeitosGameObject = new GameObject[3];
    public Image[] imagensDoStatus = new Image[3];
    public Text[] descricoesDosEfeitos = new Text[3];

    public Image imagemDoStatusCondicional;
    public Text descricaoDoEfeitoCondicional;

    public Image imagemDaImunidade;
    public Text nomeDaClasseImune;

    private List<int[]> efeitosNormais = new List<int[]>();
    private int[] imunidade = new int[2];
    private int[] efeitoCondicional;

    public void popularInformacaoDoTile(Tile tileToShow){
        for(int i = 0; i < tileToShow.efeitos.Count; i++){
            if (tileToShow.efeitos[i][0]==-1){
                imunidade = tileToShow.efeitos[i];
                imagemDaImunidade.sprite = BancoDeDados.instancia.iconesDasClasses[imunidade[0]];
                nomeDaClasseImune.text = BancoDeDados.instancia.tellMeThisClassName((Classe)imunidade[1]);
            }
            else if (tileToShow.efeitos[i].Length > 2){//Lenght > 2 == Efeito condicional
                efeitoCondicional = tileToShow.efeitos[i];
                imagemDoStatusCondicional.sprite = BancoDeDados.instancia.iconeDosStatus[efeitoCondicional[0]];
            }
        }
    }

    public void descreverEfeito(int coordenada,Efeitos efeito,int status,int porcentagem){
        descricoesDosEfeitos[coordenada].text = "";
        switch (efeito){
            case Efeitos.ClosedTerrain:                
                descricoesDosEfeitos[coordenada].text += "Campo Fechado\n";
                break;
            case Efeitos.CloseQuartersCombat:
                descricoesDosEfeitos[coordenada].text += "Combate de Curta Distância\n";
                break;
            case Efeitos.ColdTerrain:
                descricoesDosEfeitos[coordenada].text += "Terreno Frio\n";
                break;
            case Efeitos.FloodedTerrain:
                descricoesDosEfeitos[coordenada].text += "Terreno Inundado\n";
                break;
            case Efeitos.HigherGround:
                descricoesDosEfeitos[coordenada].text += "Terreno Elevado\n";
                break;
            case Efeitos.HighGround:
                descricoesDosEfeitos[coordenada].text += "Terreno Inclinado\n";
                break;
            case Efeitos.MoistTerrain:
                descricoesDosEfeitos[coordenada].text += "Terreno Úmido\n";
                break;
            case Efeitos.NarrowPassage:
                descricoesDosEfeitos[coordenada].text += "Passagem Estreita\n";
                break;
            case Efeitos.OpenTerrain:
                descricoesDosEfeitos[coordenada].text += "Campo Aberto\n";
                break;
            case Efeitos.SemiClosedTerrain:
                descricoesDosEfeitos[coordenada].text += "Campo Semi-Fechado\n";
                break;
            case Efeitos.SemiOpenTerrain:
                descricoesDosEfeitos[coordenada].text += "Campo Semi-Aberto\n";
                break;
            case Efeitos.SubZeroTerrain:
                descricoesDosEfeitos[coordenada].text += "Clima Abaixo de Zero\n";
                break;
            case Efeitos.Swamp:
                descricoesDosEfeitos[coordenada].text += "Pântano\n";
                break;
            case Efeitos.UrbanTerrain:
                descricoesDosEfeitos[coordenada].text += "Campo Urbano\n";
                break;
            case Efeitos.Woods:
                descricoesDosEfeitos[coordenada].text += "Floresta Densa\n";
                break;

        }
        descricoesDosEfeitos[coordenada].text += porcentagem + "% de " + tellMeThisStatusName(status);
    }

    public string tellMeThisStatusName(int status){
        switch (status){
            case 0: return "Pontos de Vida";
            case 1: return "Pontos de Vida Atual";
            case 2: return "Reconhecimento";
            case 3: return "Velocidade(KM/h)";
            case 4: return "Alcance";
            case 5: return "Ataque Macio";
            case 6: return "Ataque Duro";
            case 7: return "Ataque Heroico";
            case 8: return "Precisão";
            case 9: return "Esquiva";
            case 10: return "Rigidez";
            case 11: return "Perfuração";
            default: return null;
        }
    }
}
