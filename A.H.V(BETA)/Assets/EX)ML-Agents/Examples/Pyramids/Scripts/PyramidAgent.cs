using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PyramidAgent : Agent
{
    public GameObject area;
    PyramidArea m_MyArea;
    Rigidbody m_AgentRb;
    PyramidSwitch m_SwitchLogic;
    public GameObject areaSwitch;
    public bool useVectorObs;

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_MyArea = area.GetComponent<PyramidArea>();
        m_SwitchLogic = areaSwitch.GetComponent<PyramidSwitch>();
    }
    /*=========================================================================*/
    /*=========================================================================*/
    public Camera m_imgOriginal;
    public Camera m_imgWallCam;
    public Camera m_imgFloorCam;
    public Camera m_imgButtonCam;
    public Camera m_imgStoneBoxCam;
    public RenderTexture m_imgWall;
    public RenderTexture m_imgFloor;
    public RenderTexture m_imgButton;
    public RenderTexture m_imgStoneBox;
    public Material m_materialWall;
    public Material m_materialFloor;
    public Material m_materialButton;
    public Material m_materialStoneBox;
    public RenderTexture m_imgProcessed;
    public RenderTexture m_imgPassed;

    public bool m_agentCaptureTrue = false;
    // void Start(){
    //     if(GameManager.instance.m_On_Upload == true){
    //         GameManager.instance.StartImageUploader(m_imgOriginal.targetTexture);
    //     }
    // }
    void Update(){
        if(GameManager.instance.m_On_Capture == true && m_agentCaptureTrue == true){
            if(GameManager.instance.m_captureCounter > GameManager.instance.m_captureDelay){
                int fileNum = Random.Range(0, 50000);
                GameManager.instance.Shading_Texture(m_imgWallCam.targetTexture, m_imgWall, m_materialWall);
                GameManager.instance.Shading_Texture(m_imgFloorCam.targetTexture, m_imgFloor, m_materialFloor);
                GameManager.instance.Shading_Texture(m_imgButtonCam.targetTexture, m_imgButton, m_materialButton);
                GameManager.instance.Shading_Texture(m_imgStoneBoxCam.targetTexture, m_imgStoneBox, m_materialStoneBox);
                GameManager.instance.CaptureAndSaveImage(m_imgOriginal.targetTexture, fileNum, "Img\\Original\\");
                GameManager.instance.CaptureAndSaveImage(m_imgWall, fileNum, "Img\\Wall\\");
                GameManager.instance.CaptureAndSaveImage(m_imgFloor, fileNum, "Img\\Floor\\");
                GameManager.instance.CaptureAndSaveImage(m_imgButton, fileNum, "Img\\Button\\");
                GameManager.instance.CaptureAndSaveImage(m_imgStoneBox, fileNum, "Img\\StoneBox\\");
                Debug.Log(fileNum);
            }
        }
        if(GameManager.instance.m_On_Upload == true){
            if(GameManager.instance.m_captureCounter > GameManager.instance.m_captureDelay){
                GameManager.instance.StartImageUploader(m_imgOriginal.targetTexture, m_imgProcessed);
                GameManager.instance.Passing_Texture(m_imgOriginal.targetTexture, m_imgPassed);     

            }
            // GameManager.instance.Passing_Texture(m_imgOriginal.targetTexture, m_imgProcessed);             
        }
        else{
            GameManager.instance.Passing_Texture(m_imgOriginal.targetTexture, m_imgProcessed);     
        }
        
    }
    /*=========================================================================*/
    /*=========================================================================*/
    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                // dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
        }
        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        m_AgentRb.AddForce(dirToGo * 2f, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)

    {
        AddReward(-1f / MaxStep);
        MoveAgent(actionBuffers.DiscreteActions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        // else if (Input.GetKey(KeyCode.S))
        // {
        //     discreteActionsOut[0] = 2;
        // }
    }

    public override void OnEpisodeBegin()
    {
        var enumerable = Enumerable.Range(0, 9).OrderBy(x => Guid.NewGuid()).Take(9);
        var items = enumerable.ToArray();

        m_MyArea.CleanPyramidArea();

        m_AgentRb.velocity = Vector3.zero;
        m_MyArea.PlaceObject(gameObject, items[0]);
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

        m_SwitchLogic.ResetSwitch(items[1], items[2]);
        m_MyArea.CreateStonePyramid(1, items[3]);
        m_MyArea.CreateStonePyramid(1, items[4]);
        m_MyArea.CreateStonePyramid(1, items[5]);
        m_MyArea.CreateStonePyramid(1, items[6]);
        m_MyArea.CreateStonePyramid(1, items[7]);
        m_MyArea.CreateStonePyramid(1, items[8]);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("goal"))
        {
            SetReward(2f);
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("switchOff"))
        {
            SetReward(2f);
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("switchOn"))
        {
            SetReward(2f);
            EndEpisode();
        }
    }
}
