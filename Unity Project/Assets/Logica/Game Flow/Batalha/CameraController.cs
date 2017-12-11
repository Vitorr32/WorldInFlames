using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {
    public static CameraController instancia;
    
    void Awake(){
        instancia= this;
    }

    public float dragSpeed = 0.2f;
    public float velocidaDaCamera = 5f;

    public float maximoOrtopathic = 15;
    public float minimoOrtopathic = 1;

    public bool onDrag;//Se o player está atualmente usando o drag, fazendo a camerâ desfocar no movimento do player atual
    private Vector3 dragOrigin;

    private Vector3 cantoSuperiorDireito;
    private Vector3 cantoInferiorEsquerdo;

    //private Vector3 cantoSuperiorDireito;
    //private Vector3 cantoInferiorEsquerdo;

    public bool cameraDragging = true;

    public Vector3 boardCenter;
    public List<List<Tile>> tabuleiro;

    public bool tabuleiroCriado = false;
    
    void LateUpdate(){
        if (tabuleiroCriado){
            //Mecanica do Zoom On e Off
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && this.GetComponent<Camera>().orthographicSize < maximoOrtopathic){
                this.GetComponent<Camera>().orthographicSize = Mathf.Max(this.GetComponent<Camera>().orthographicSize+1, minimoOrtopathic);
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && this.GetComponent<Camera>().orthographicSize> minimoOrtopathic){
                this.GetComponent<Camera>().orthographicSize = Mathf.Min(this.GetComponent<Camera>().orthographicSize-1, maximoOrtopathic);
            }
            //Mecanica do drag da camera
            if (Input.GetMouseButton(1)){
                onDrag = true;
                float x = Input.GetAxis("Mouse X") * dragSpeed;
                float y = Input.GetAxis("Mouse Y") * dragSpeed;
                transform.Translate(x, y, 0);
            }
            else{
                onDrag = false;
            }

            if (Input.GetMouseButton(2)){
                olharProCentroDoTabuleiro();
            }

            /*if (BatalhaController.instancia.unidadesNoTabuleiro[BatalhaController.instancia.unidadeAtualIndex].movendo){
                focarNoPlayer(BatalhaController.instancia.unidadesNoTabuleiro[BatalhaController.instancia.unidadeAtualIndex]);
            }*/

            //Checar se está no limite do cenario
            if(transform.position.x > cantoSuperiorDireito.x){
                transform.position = new Vector3(cantoSuperiorDireito.x, transform.position.y, transform.position.z);
            }
            else if(transform.position.x < cantoInferiorEsquerdo.x) {
                transform.position = new Vector3(cantoInferiorEsquerdo.x, transform.position.y, transform.position.z);
            }

            if(transform.position.z > cantoSuperiorDireito.z){
                transform.position = new Vector3(transform.position.x, transform.position.y, cantoSuperiorDireito.z);
            }
            else if (transform.position.z < cantoInferiorEsquerdo.z){
                transform.position = new Vector3(transform.position.x, transform.position.y, cantoInferiorEsquerdo.z);
            }
        }
    }

	public void calcularLimitesDaCamera(){
        //+X e direita, -X e esquerda, +Z é cima,-Z e baixo
		int colunas = tabuleiro.Count;
		int linhas = tabuleiro[0].Count;

        cantoInferiorEsquerdo = tabuleiro[0][linhas-1].transform.position + new Vector3(-3,0,-3);
        cantoSuperiorDireito = tabuleiro[colunas-1][0].transform.position + new Vector3(3,0,3);

        tabuleiroCriado = true;
    }

    /*public void focarNoPlayerAtual() {
        transform.position = BatalhaController.instancia.unidadesNoTabuleiro[BatalhaController.instancia.unidadeAtualIndex].transform.position + Vector3.up *5;
    }*/

    public void setarTabuleiro(List<List<Tile>> tabuleiro){
        this.tabuleiro = tabuleiro;
    }

    public void focarNoPlayer(UnidadeNoTabuleiro playerAFocar) {
        transform.position = playerAFocar.transform.position + Vector3.up * 5;
        //this.GetComponent<Camera>().orthographicSize = 2;
    }

    public IEnumerator focarNaPosicaoSuave(Transform posicaoDoPlayer){
        Vector3 posicaoAlvo = posicaoDoPlayer.position + Vector3.up * 5;
        while (transform.position != posicaoAlvo){
            transform.position += (posicaoAlvo - transform.position).normalized * velocidaDaCamera * Time.deltaTime;
            if (Vector3.Distance(posicaoAlvo, transform.position) <= 0.1f){
                transform.position = posicaoAlvo;
                yield break;
            }
        }
    }

    public void setOrthopaicSize(int newSize){
        this.GetComponent<Camera>().orthographicSize = newSize;
    }

    public void olharProCentroDoTabuleiro(){
        boardCenter = tabuleiro[tabuleiro.Count / 2][tabuleiro[0].Count / 2].transform.position + Vector3.up * 5;
        transform.position = boardCenter;
        this.GetComponent<Camera>().orthographicSize = 5;
    }

    public void mudandoDeFase(){
        tabuleiroCriado = false;
    }
}
