using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class InterfaceDeConstrucaoDeMapaController : MonoBehaviour {
    public bool salvandoOuCarregando;
    public GameObject InputFieldHolder;
    public GameObject InputFieldHolderCoordenadas;
    public GameObject unidadesHolder;
    public GameObject botaoComumPrefab;
    public InputField campoDeInput;
    public InputField inputX;
    public InputField inputY;
    public Text feedbackSalvarOuCarregar;
    public Text feedbackCoordenadas;

    void Start(){
        PopularUnidades();
    }

    public void PopularUnidades(){
        foreach(Unidade u in BancoDeDados.instancia.unidadesDoJogo){
            if (u.nacionalidade == Pais.Nevoa){
                GameObject botaoNovo = (GameObject)Instantiate(botaoComumPrefab);
                botaoNovo.GetComponentInChildren<Text>().text = u.nome;
                int id = u.idNoBancoDeDados;
                botaoNovo.GetComponent<Button>().onClick.AddListener(() => SelecionarUnidades(id));
                botaoNovo.transform.SetParent(unidadesHolder.transform, false);
                botaoNovo.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public void SelecionarUnidades(int idDaUnidade){
        if (MapCreatorManager.instance.unidadeSendoSetada != null) { Destroy(MapCreatorManager.instance.unidadeSendoSetada); }
        MapCreatorManager.instance.colocandoUnidade = true;
        MapCreatorManager.instance.mudandoMecanica = false;
        MapCreatorManager.instance.mudandoTipo = false;
        MapCreatorManager.instance.instanciarUnidade(idDaUnidade);
    }
   
    public void mudarMecanicaDeTile(int qual){
        if (MapCreatorManager.instance.unidadeSendoSetada != null) { Destroy(MapCreatorManager.instance.unidadeSendoSetada); }
        switch (qual){
            case 0:
                MapCreatorManager.instance.mechanicPallet = TileMechanic.NO_MECHANIC;
                break;
            case 1:
                MapCreatorManager.instance.mechanicPallet = TileMechanic.PLAYER_ATTACK_SUPPORT;
                break;
            case 2:
                MapCreatorManager.instance.mechanicPallet = TileMechanic.PLAYER_RETREAT_LINE;
                break;
            case 3:
                MapCreatorManager.instance.mechanicPallet = TileMechanic.PLAYER_SPAWN_REINFORCEMENT;
                break;
            case 4:
                MapCreatorManager.instance.mechanicPallet = TileMechanic.PLAYER_SPAWN_MAIN;
                break;
            case 5:
                MapCreatorManager.instance.mechanicPallet = TileMechanic.ENEMY_RETREAT_LINE;
                break;
            case 6:
                MapCreatorManager.instance.mechanicPallet = TileMechanic.ENEMY_SPAWN_REINFORCEMENT;
                break;
            case 7:
                MapCreatorManager.instance.mechanicPallet = TileMechanic.ENEMY_SPAWN_SUPPORT;
                break;
        }
        MapCreatorManager.instance.mudandoMecanica = true;
        MapCreatorManager.instance.mudandoTipo = false;
        MapCreatorManager.instance.colocandoUnidade = false;
    }
        
    public void mudarPaleta(int qual) {
        if (MapCreatorManager.instance.unidadeSendoSetada != null) { Destroy(MapCreatorManager.instance.unidadeSendoSetada);}
        switch(qual){
            case 0:
                MapCreatorManager.instance.palletSelection = TileType.Rua;
            break;
            case 1:
                MapCreatorManager.instance.palletSelection = TileType.Plano;
            break;
            case 2:
                MapCreatorManager.instance.palletSelection = TileType.Terra;
            break;
            case 3:
                MapCreatorManager.instance.palletSelection = TileType.Deserto;
            break;
            case 4:
                MapCreatorManager.instance.palletSelection = TileType.NeveLeve;
            break;
            case 5:
                MapCreatorManager.instance.palletSelection = TileType.NeveFunda;
            break;
            case 6:
                MapCreatorManager.instance.palletSelection = TileType.Colina;
            break;
            case 7:
                MapCreatorManager.instance.palletSelection = TileType.Montanha;
            break;
            case 8:
                MapCreatorManager.instance.palletSelection = TileType.Rio;
            break;
            case 9:
                MapCreatorManager.instance.palletSelection = TileType.GrandeRio;
            break;
            case 10:
                MapCreatorManager.instance.palletSelection = TileType.Ponte;
            break;
            case 11:
                MapCreatorManager.instance.palletSelection = TileType.Floresta;
            break;
            case 12:
                MapCreatorManager.instance.palletSelection = TileType.Pantano;
            break;
            case 13:
                MapCreatorManager.instance.palletSelection = TileType.Urbano;
            break;
            case 14:
                MapCreatorManager.instance.palletSelection = TileType.Null;
            break;
        }
        MapCreatorManager.instance.mudandoMecanica = false;
        MapCreatorManager.instance.mudandoTipo = true;
        MapCreatorManager.instance.colocandoUnidade = false;
    }

    public void abrirTelaDeInput(bool salvarOuCarregar){
        fecharTelaDeInputDeCoordenadas();
        fecharTelaDeInputDeCoordenadas();
        salvandoOuCarregando= salvarOuCarregar;
        if(!InputFieldHolder.activeSelf)InputFieldHolder.SetActive(true);
        feedbackSalvarOuCarregar.text="Escreva o nome do mapa a ser salvo/carregado, não digite .xml aqui";
    }

    public void fecharTelaDeInput(){
        if(InputFieldHolder.activeSelf)InputFieldHolder.SetActive(false);
    }

    public void fecharTelaDeInputDeCoordenadas(){
        if(InputFieldHolderCoordenadas.activeSelf)InputFieldHolder.SetActive(false);
    }

    public void abrirTelaDeInputDeCoordenadas(){
        fecharTelaDeInput();
        if(!InputFieldHolderCoordenadas.activeSelf)InputFieldHolderCoordenadas.SetActive(true);
        feedbackCoordenadas.text="Escreva o tamnho de linhas (x) e o tamanho de colunas(y)";
    }

    public void SalvarOuCarregar(){
        if(campoDeInput.text.Equals(null)){
            feedbackSalvarOuCarregar.text="O arquivo a carregar/salvar deve ter um nome, não acha?";
        }
        else{
            if(salvandoOuCarregando){
                MapCreatorManager.instance.saveMapToXml(campoDeInput.text);
            }
            else{
                MapCreatorManager.instance.loadMapFromXml(campoDeInput.text);
            }
        }
        feedbackSalvarOuCarregar.text="";
        campoDeInput.text="";
        fecharTelaDeInput();
    }

    public void GerarNovoMapa(){
        if(inputX.text.Equals(null) || inputY.text.Equals(null)){
            feedbackSalvarOuCarregar.text="Você deve digitar o tamanho coordenadas";
        }
        else{
            MapCreatorManager.instance.generateBlankMap(Int32.Parse(inputX.text), Int32.Parse(inputY.text));
        }
        feedbackCoordenadas.text="";
        inputX.text="";
        inputY.text="";
        InputFieldHolderCoordenadas.SetActive(false);
    }
}
