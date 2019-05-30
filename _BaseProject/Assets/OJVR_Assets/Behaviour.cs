using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIEventDelegate;

public class Behaviour : MonoBehaviour
{
    public string mySignal = "C3"; //signal to respond to
    public string false_trigger = "space";
   public bool useTimer = false;
    public int repeats = 10; // amount of times alternate action runs
    public float duration = 0.5f; // duration of alternate action
    

    public EventDelegate myMainAction;
    public EventDelegate myAltAction;


//    public String mainAction = "Actions.moveForward";
//    public String alternateAction = "Actions.moveBackward";


    int repeatCount = 0;
    bool started = false;
    float timer = 0;
    private TimingsManager time_manager;
    GameObject time_manager_gameobject;

    

    int ID;

    void Start(){
      //set up ID
      print("starting");
      time_manager_gameobject = GameObject.FindWithTag("TimingManager");
      time_manager = time_manager_gameobject.GetComponent<TimingsManager>();
        
      ID = time_manager.getUniqueID();
    }

    //should alternate action be done?
    bool ifDoAlternate(){
      if(falseSignal() || signal()){
        started = true;
      }
      if(started){
        repeatCount++;
        if(useTimer){
          timer = timer + 1 * Time.deltaTime;
          if(timer > duration){
            started = false;
            return false;
          }
        }else{
          if(repeatCount > repeats){
            started = false;
            return false;
          }
        }
      
        return true;
      }else{
        timer = 0;
        repeatCount = 0;
      }
      return false;

    }

    bool falseSignal(){
      if(Input.GetKeyDown(false_trigger)){
        return true;
      }else{
        return false;
      }
    }

    bool signal(){
      return time_manager.hasSignal(mySignal, ID);
    }

    void Update(){
      if(!ifDoAlternate()){
        normalAction();
      }else{
        alternateAction();
      }
    }
    void normalAction(){
      myMainAction.Execute();/////////////////////////////////////
    }

    void alternateAction(){
      myAltAction.Execute();
    //  Actions.multiply(this.gameObject,25);/////////////////////////////////////
    }


}
