using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{
    //simple movement
    public void moveForward(GameObject go, float speed){
       go.transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    public void moveBackward(GameObject go,float speed){
       go.transform.Translate(Vector3.forward * Time.deltaTime * speed * -1);
    }

    //rotation
    public void rotateRandom(GameObject go){
      go.transform.rotation = Random.rotation;
    }

    public void multiply(GameObject go, int num){
      for(int i = 0; i < num; i++){
        Instantiate(go, go.transform);
        Destroy(go);
      }
    }
}
