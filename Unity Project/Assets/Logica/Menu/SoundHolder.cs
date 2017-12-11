using UnityEngine;
using System.Collections;

public class SoundHolder : MonoBehaviour {
    public static SoundHolder instancia;
    public AudioSource fonteDeSom;

    public AudioClip whispering;
    public AudioClip busyCity;
    public AudioClip militaryAlarm;

    public AudioClip botaoHoverMetal;
    public AudioClip botaoClickMetal;
    public AudioClip botaoClickStamp;
    public AudioClip botaoClickPapel;

    public AudioClip ArsenalEquipmentSelect;

    public AudioClip SoldiersMarch;
    public AudioClip TanqueCharge;
    public AudioClip Construcition;
    public AudioClip BoilingWater;
    public AudioClip RunningAway;
    public AudioClip Recharging;

    //Ataques
    public AudioClip ArtilleryStrike;
    public AudioClip AssaultRifleGunfight;
    public AudioClip SniperQuickScope;
    public AudioClip RifleShootingGunfight;
    public AudioClip TankTurrerStrike;
    public AudioClip Crossbow;
    public AudioClip CavalaryCharge;
    public AudioClip ArmaDeCercoMedieval;
    public AudioClip MedievalInfantry;
    public AudioClip FogLordAttack;
    //public AudioClip TankMachineGun;
    //public AudioClip RocketLauncher;
    //Defesas
    /*
    public AudioClip bulletsHit;
    public AudioClip bulletsOnHardHit;
    public AudioClip shellHit;
    public AudioClip heroicTarget;
    public AudioClip bulletMiss;
    public AudioClip shellMiss;
    public AudioClip artilleryHit;
    */
    void Start(){
        instancia = this;
    }

    public void HoverDeBotaoMetalico(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(botaoHoverMetal);
    }

    public void ClickDeBotaoMetalico(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(botaoClickMetal);
    }

    public void ClickEmBotaoDeStamp(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(botaoClickStamp);
    }
    public void ClickEmBotaoDePapel(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(botaoClickPapel);
    }
    public void SelecaoEquipamentoArsenal(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(ArsenalEquipmentSelect);
    }
    public void MarchaDeSoldados(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(SoldiersMarch);
    }
    public void TanqueAvanco(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(TanqueCharge);
    }
    public void Construcao(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(Construcition);
    }
    public void AguaFervendo(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(BoilingWater);
    }
    public void AlarmeMilitar(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(militaryAlarm);
    }
    public void sussuros(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(whispering);
    }
    public void cidadeNormal(){
        fonteDeSom.Stop();
        fonteDeSom.PlayOneShot(busyCity);
    }
}
