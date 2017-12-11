using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CartaEquipamentoComparacao : MonoBehaviour {
    public Text nomeDoEquipamentoAtual;
    public Text statusDoEquipamentoAtual;
    public Image classeEquipamentoIconeAtual;

    public Text nomeDoEquipamentoNovo;
    public Text statusDoEquipamentoNovo;
    public Image classeEquipamentoIconeNovo;

    public GameObject holderDeIconesAtual;
    public GameObject holderDeIconesNovo;

    public Sprite[] icones;
    public Sprite nothingSprite;
 

    public void popularEquipamentoParaMudanca(Equipamento equipamentoNovo, Unidade unidadeAlvo, int aTrocar){
        limparComparador();
        bool equipamentoAtualExiste = false;

        if (unidadeAlvo.equipamento.Count > aTrocar && !unidadeAlvo.equipamento[aTrocar].Equals(null)){
            classeEquipamentoIconeAtual.sprite = BancoDeDados.instancia.iconesDeTiposDeEquipamentos[(int)unidadeAlvo.equipamento[aTrocar].tipoDeEquipamento];
            nomeDoEquipamentoAtual.text = unidadeAlvo.equipamento[aTrocar].nome;
            equipamentoAtualExiste = true;
        }
        else{
            classeEquipamentoIconeAtual.sprite = nothingSprite;
            nomeDoEquipamentoAtual.text = "";
        }

        classeEquipamentoIconeNovo.sprite = BancoDeDados.instancia.iconesDeTiposDeEquipamentos[(int)equipamentoNovo.tipoDeEquipamento];
        nomeDoEquipamentoNovo.text = equipamentoNovo.nome;

        int statusUnicosAtual = 0;
        int statusUnicosNovo = 0;     

        for(int i = 0; i < 11; i++){            
            if(equipamentoAtualExiste && unidadeAlvo.equipamento[aTrocar].statusDoEquipamento[i] != 0){
                adicionarStatusAoComparador(holderDeIconesAtual, statusDoEquipamentoAtual, i, unidadeAlvo.equipamento[aTrocar].statusDoEquipamento[i], statusUnicosAtual);
                statusUnicosAtual++;
            }
            if (equipamentoNovo.statusDoEquipamento[i] != 0){
                adicionarStatusAoComparador(holderDeIconesNovo, statusDoEquipamentoNovo, i, equipamentoNovo.statusDoEquipamento[i], statusUnicosNovo);
                statusUnicosNovo++;
            }
        }

        /*
        int[] statusAtual = unidadeAlvo.status;
        int[] statusComNovoEquipamento = unidadeAlvo.calcularStatusComEquipamentoTrocado(equipamentoNovo,aTrocar);


        for(int i=0;i<11;i++){
            Debug.Log("Estamos no status "+i+" o atual é "+statusAtual[i]+ " enquanto o novo seria "+ statusComNovoEquipamento[i]);
            if(statusAtual[i]!=statusComNovoEquipamento[i]){
                Debug.Log("Portanto ele entrou no if");
                GameObject novoComparador = Instantiate(comparadorPorStatusPrefab);
                novoComparador.transform.SetParent(this.transform);
                if(statusAtual[i]>statusComNovoEquipamento[i]){
                    novoComparador.GetComponentInChildren<Text>().text = "+"+(statusAtual[i]-statusComNovoEquipamento[i]);
                    novoComparador.GetComponentInChildren<Text>().color = Color.green;
                }
                else{
                    novoComparador.GetComponentInChildren<Text>().text = (statusAtual[i]-statusComNovoEquipamento[i]).ToString();
                    novoComparador.GetComponentInChildren<Text>().color = Color.red;
                }
                setarIconeDoStatus(novoComparador.transform.GetChild(0).GetComponent<Image>(),i);
                novoComparador.transform.SetParent(comparadoresHolder.transform);
            }
        }
        */
    }

    public void limparComparador(){
        if (statusDoEquipamentoAtual.text.Length != 0){
            statusDoEquipamentoAtual.text = "";
        }
        for(int i = 0; i < holderDeIconesAtual.transform.childCount; i++){
            holderDeIconesAtual.transform.GetChild(i).GetComponent<Image>().sprite = nothingSprite;
        }
        if (statusDoEquipamentoNovo.text.Length != 0){
            statusDoEquipamentoNovo.text = "";
        }
        for (int i = 0; i < holderDeIconesNovo.transform.childCount; i++){
            holderDeIconesNovo.transform.GetChild(i).GetComponent<Image>().sprite = nothingSprite;
        }
    }

    public void adicionarStatusAoComparador(GameObject holderDeIcones,Text statusAMecher,int qualStatus,int valorAMostrar,int qualSlot) {
        switch (qualStatus){
            case 0:
                holderDeIcones.transform.GetChild(qualSlot).GetComponent<Image>().sprite = retornarIconeDoStatus(0);
                statusAMecher.text += "Pontos de Saude ";                
                break;
            case 2:
                holderDeIcones.transform.GetChild(qualSlot).GetComponent<Image>().sprite = retornarIconeDoStatus(2);
                statusAMecher.text += "Reconhecimento ";
                break;
            case 3:
                holderDeIcones.transform.GetChild(qualSlot).GetComponent<Image>().sprite = retornarIconeDoStatus(3);
                statusAMecher.text += "Velocidade ";
                break;
            case 4:
                holderDeIcones.transform.GetChild(qualSlot).GetComponent<Image>().sprite = retornarIconeDoStatus(4);
                statusAMecher.text += "Alcance ";
                break;
            case 5:
                holderDeIcones.transform.GetChild(qualSlot).GetComponent<Image>().sprite = retornarIconeDoStatus(5);
                statusAMecher.text += "Ataque Macio ";
                break;
            case 6:
                holderDeIcones.transform.GetChild(qualSlot).GetComponent<Image>().sprite = retornarIconeDoStatus(6);
                statusAMecher.text += "Ataque Pesado ";
                break;
            case 7:
                holderDeIcones.transform.GetChild(qualSlot).GetComponent<Image>().sprite = retornarIconeDoStatus(7);
                statusAMecher.text += "Ataque Heroico ";
                break;
            case 8:
                holderDeIcones.transform.GetChild(qualSlot).GetComponent<Image>().sprite = retornarIconeDoStatus(8);
                statusAMecher.text += "Precisão ";
                break;
            case 9:
                holderDeIcones.transform.GetChild(qualSlot).GetComponent<Image>().sprite = retornarIconeDoStatus(9);
                statusAMecher.text += "Esquiva ";
                break;
            case 10:
                holderDeIcones.transform.GetChild(qualSlot).GetComponent<Image>().sprite = retornarIconeDoStatus(10);
                statusAMecher.text += "Rigidez ";
                break;
            case 11:
                holderDeIcones.transform.GetChild(qualSlot).GetComponent<Image>().sprite = retornarIconeDoStatus(11);
                statusAMecher.text += "Perfuração ";
                break;
        }
        if (valorAMostrar > 0) { statusAMecher.text += "+" + valorAMostrar + "\n"; }
        else { statusAMecher.text += valorAMostrar + "\n"; }
    }

    public Sprite retornarIconeDoStatus(int qual){
        if(qual==0) {
            return icones[0];
        }
        else{
            return icones[qual-1];
        }
    }
}
