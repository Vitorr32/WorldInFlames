using UnityEngine;
using System.Collections;

public class SoundPlayer : MonoBehaviour {
    public static SoundPlayer instancia;
    public AudioSource fonteDeSom;

    void Start(){
        instancia = this;
    }

    public void HoverDeBotaoMetalico(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.botaoHoverMetal);
    }

    public void ClickDeBotaoMetalico(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.botaoClickMetal);
    }

    public void ClickEmBotaoDeStamp(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.botaoClickStamp);
    }
    public void ClickEmBotaoDePapel(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.botaoClickPapel);
    }
    public void SelecaoEquipamentoArsenal(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.ArsenalEquipmentSelect);
    }
    public void MarchaDeSoldados(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.SoldiersMarch);
    }
    public void TanqueAvanco(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.TanqueCharge);
    }
    public void Construcao(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.Construcition);
    }
    public void AguaFervendo(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.BoilingWater);
    }
    public void Vazando(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.RunningAway);
    }
    public void Recarregando(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoundHolder.instancia.Recharging);
    }
}
