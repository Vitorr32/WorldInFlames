using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EventosDoHQ : MonoBehaviour {
    public static EventosDoHQ instancia;

    public AudioSource fonteDeSom;

    public GameObject arsenal;
    public GameObject quartel;
    public GameObject campanhas;
    public GameObject linhasDeProducao;
    public GameObject mordenizacao;   

    public Text[] recursos = new Text[5];
    
    void Awake(){
        instancia = this;
    }

	void Start () {
        atualizarInterface();
    }

    public void SalvarJogo(){
        GameController.SaveLoad.Save();
    }

    public void voltarProMainMenu(){
        Destroy(GameController.instancia.gameObject);
        SceneManager.LoadScene("Menu");
    }

     public void atualizarInterface(){
        recursos[0].text = GameController.instancia.Combustivel.ToString();
        recursos[1].text = GameController.instancia.Municao.ToString();
        recursos[2].text = GameController.instancia.MaoDeObra.ToString();
        recursos[3].text = GameController.instancia.Metal.ToString();
        recursos[4].text = GameController.instancia.Inspiracao.ToString();
    }

    public void VoltarParaMain(){
        esconderQuartel();
        esconderArsenal();
        esconderCampanhas();

        linhasDeProducao.GetComponent<LinhasDeProducaoController>().EstadoOriginal();
        esconderLinhas();

        esconderMordenizacao();
        atualizarInterface();
    }    
    public void mostrarLinhas(){
        VoltarParaMain();
        linhasDeProducao.SetActive(true);
    }
    public void esconderLinhas(){
        linhasDeProducao.SetActive(false);
    }
    public void mostrarMordenizacao() {
        VoltarParaMain();
        mordenizacao.SetActive(true);
    }
    public void esconderMordenizacao(){
        mordenizacao.SetActive(false);
    }
    public void mostrarCampanhas(){
        VoltarParaMain();
        campanhas.SetActive(true);
    }
    public void esconderCampanhas(){
        campanhas.SetActive(false);
    }
    public void mostrarQuartel(){
        VoltarParaMain();
        quartel.SetActive(true);
    }
    public void esconderQuartel(){
        quartel.SetActive(false);
    }
    public void mostrarArsenal(){
        VoltarParaMain();
        arsenal.SetActive(true);
    }
    public void esconderArsenal(){
        arsenal.SetActive(false);
    }
    public void HoverDeBotaoMetalico()
    {
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.botaoHoverMetal);
    }

    public void ClickDeBotaoMetalico()
    {
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.botaoClickMetal);
    }

    public void ClickEmBotaoDeStamp()
    {
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.botaoClickStamp);
    }
    public void ClickEmBotaoDePapel()
    {
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.botaoClickPapel);
    }
    public void SelecaoEquipamentoArsenal()
    {
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.ArsenalEquipmentSelect);
    }
    public void RecargaDeSurpimentos(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.Recharging);
    }
    public void MarchaDeSoldados()
    {
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.SoldiersMarch);
    }
    public void TanqueAvanco()
    {
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.TanqueCharge);
    }
    public void Construcao()
    {
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.Construcition);
    }
    public void AguaFervendo()
    {
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.BoilingWater);
    }
}
