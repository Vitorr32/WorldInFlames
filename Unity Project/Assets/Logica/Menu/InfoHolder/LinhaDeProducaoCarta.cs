using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LinhaDeProducaoCarta : MonoBehaviour { //Guarda as informações da carta da linha de produção, podendo ser destruida sem perder a linha
    public int slot;//Qual slot esta linha se encontra na lista do GameController

    public LinhaDeProducao linhaAssociada;//Qual linha de produção está associada a essa carta

    public Text ocupadotexto;//Text UI que mostra o estado atual da linha, ocupada ou livre
    public Text descricaoProducao;//Descrição da produção atual
    public Text proguessoNumero;//Text UI que mostra o proguesso atual
    public Image imagemProducao;//Imagem da produção atuak
    public Button trocarProducao;//Botao que carregas as ações de trocar/coletar/produzir
    public Slider barraDeProguesso;//Slider UI que controla a barra de proguesso
    public Image imagemBaseBotao;//Imagem base do botão de trocar/coletar/produzir    
    
    public Animator[] engrenagens = new Animator[2];//Contém os animator para a animação das engrenagens

    void Start(){
        barraDeProguesso.value=0;
    }
}
