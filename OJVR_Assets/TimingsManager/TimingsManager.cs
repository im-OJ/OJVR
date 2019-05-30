using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using SimpleJSON;
public class TimingsManager : MonoBehaviour
{

    public string json_file_path;
    AudioSource audiosource;
    int start_time;
    int current_time;
    string text;
    public bool muted = false;

    public float signal_total_lifetime = 0.005f;

    //SIGNAL ARRAY LISTS
    ArrayList signal_string = new ArrayList();
    ArrayList signal_time = new ArrayList();

    ArrayList signal_hasfired = new ArrayList();
    ArrayList signal_life = new ArrayList();
    ArrayList active_signals = new ArrayList();
    ArrayList signal_has_reset = new ArrayList();

    //
    ArrayList has_received = new ArrayList(); 
  
    void Start(){
        text = System.IO.File.ReadAllText(json_file_path);
        //N = JSON.parse(text);
        
        audiosource = GetComponent<AudioSource>();
        start_time = (int) System.DateTime.Now.Ticks;
        if(!muted){
            audiosource.Play(0);
        }
        setUpStructure();
        
       
    }
    int cID = 0;
    public int getUniqueID(){
        cID++;
        return cID;   
    }

    // Update is called once per frame
    void Update(){

        //Check all signals (TODO: could be done faster with pre-sorting)
        current_time = (int) System.DateTime.Now.Ticks - start_time;
        active_signals = new ArrayList();
        
        for(int key = 0; key < signal_string.Count; key++){
            float time_sec = (float) signal_time[key];
            
            int time_ticks = (int) secondsToTicks(time_sec);
            string text = (string) signal_string[key];
            bool hasfired = (bool) signal_hasfired[key];
           
            if(time_ticks < current_time){
                 //if signals time has passed
                if(!(bool) signal_hasfired[key]){
                    Debug.Log("firing signal: " + text + " at " + time_sec);
                    
                    signal_hasfired[key] = true;
                }
               //if signal alive
               if((bool)signal_hasfired[key]){
                   //if signal alive
                    if((float)signal_life[key] > 0f){
                        signal_life[key] = (float) signal_life[key] - Time.deltaTime;
                        active_signals.Add(signal_string[key]);
                        if(!(bool)signal_has_reset[key]){
                            has_received = new ArrayList();
                            signal_has_reset[key] = true;
                        }
                        
                    }
                }


                


            }

        }


    }



  
    public bool hasSignal(string signalname, int ID){
       
       if(has_received.Contains(ID)){
           return false;
       }

       if(active_signals.Contains(signalname)){
           has_received.Add(ID);
           return true;
       }else{
           return false;
       }

    }
    void setUpStructure(){
        var N = JSON.Parse(text);
        float time_gone = Time.time - start_time;

        foreach(var note in  N){

            foreach( var item in note.Value){
                signal_string.Add((string)note.Key);
                signal_time.Add((float)item.Value["time"]);
                signal_hasfired.Add(false);
                signal_life.Add(signal_total_lifetime);
                signal_has_reset.Add(false);
            }
    
         }
        Debug.Log(N[0][1][0]);
       

    }


    public int secondsToTicks(float seconds){
        seconds = seconds * 10000000;
        return (int) seconds;
    }




}
