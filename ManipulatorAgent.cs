using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;
using MLAgents.SideChannels;
using Barracuda;
using TMPro;
using System;
using System.Threading;






public class ManipulatorAgent : Agent
{

    public GameObject part1, part2, part3, part4, part5, part6, part61, part62, target1, target2, target3, target4, target5, zone;
    public GameObject b1, b2, b3, b4, b5, b6, b7, b8;
   //public CollisionGameObjectExample collision;
    public TextMeshPro rewardText;
    public float actionSpeed = 10.0f;
    public float speed;
    public float part2_min =  0;
    public float part2_max =  90;
    public float part3_min = -150;
    public float part3_max =  0;
    public float part5_min = -100;
    public float part5_max =  100;
    public float end_min = 1;
    public float end_max =  60;
    public float zone_radius = 650;

    private float lastDistTarget1ToZone = 1000000;
    private float lastDistTarget2ToZone = 1000000;
    private float lastDistTarget3ToZone = 1000000;
    private float lastDistTarget4ToZone = 1000000;
    private float lastDistTarget5ToZone = 1000000;

    private float lastDistEndEffectorToTarget1 = 1000000;
    private float lastDistEndEffectorToTarget2 = 1000000;
    private float lastDistEndEffectorToTarget3 = 1000000;
    private float lastDistEndEffectorToTarget4 = 1000000;
    private float lastDistEndEffectorToTarget5 = 1000000;

    private float distEndEffectorToTarget1;
    private float distEndEffectorToTarget2;
    private float distEndEffectorToTarget3;
    private float distEndEffectorToTarget4;
    private float distEndEffectorToTarget5;

    private float distTarget2ToZone;
    private float distTarget3ToZone;
    private float distTarget4ToZone;
    private float distTarget1ToZone;
    private float distTarget5ToZone;

    private bool useTarget1 = true;
    private bool useTarget2 = false;
    private bool useTarget3 = false;
    private bool useTarget4 = false;
    private bool useTarget5 = false;
    private bool useCurricula = true;
    bool collided = false;

    private float posRewardApproachTarget = 0.03f;
    private float negRewardApproachTarget = -0.05f;
    private bool doUpdate = false;
    private float posRewardPushTarget = 0.2f;
    private float negRewardPushTarget = -0.5f;
    FloatPropertiesChannel m_FloatProperties;

    public override void Initialize()
    {
        //SideChannelUtils.GetSideChannel<FloatPropertiesChannel>();
    }

    private void Update()
    {
        // Debug.Log(border.transform.position[1] - part1.transform.position[1]);
        if (b1.transform.position[1] - part1.transform.position[1] > 0 ||
            b2.transform.position[1] - part1.transform.position[1] > 0 ||
            b3.transform.position[1] - part1.transform.position[1] > 0 ||
            b4.transform.position[1] - part1.transform.position[1] > 0 ||
            b5.transform.position[1] - part1.transform.position[1] > 0 ||
            b6.transform.position[1] - part1.transform.position[1] > 0 ||
            b7.transform.position[1] - part1.transform.position[1] > 0 ||
            b8.transform.position[1] - part1.transform.position[1] > 0)
        {
            Debug.Log("collided");
            AddReward(-10.0f);
            EndEpisode();
        }

        
        if (doUpdate)
        {
            //useTarget2 = (Academy.Instance.FloatProperties.GetPropertyWithDefault("secondObject", 0) != 0);
            //Debug.Log("Update called");
            AddReward(-0.05f);
            
            //if (this.GetCumulativeReward() < - 10) EndEpisode();
            distEndEffectorToTarget1 = Vector3.Distance(part6.transform.position, target1.transform.position);
            distEndEffectorToTarget2 = Vector3.Distance(part6.transform.position, target2.transform.position);
            distEndEffectorToTarget3 = Vector3.Distance(part6.transform.position, target3.transform.position);
            distEndEffectorToTarget4 = Vector3.Distance(part6.transform.position, target4.transform.position);
            distEndEffectorToTarget5 = Vector3.Distance(part6.transform.position, target5.transform.position);

            distTarget1ToZone = flatDistance(target1.transform.position, zone.transform.position);
            distTarget2ToZone = flatDistance(target2.transform.position, zone.transform.position);
            distTarget3ToZone = flatDistance(target3.transform.position, zone.transform.position);
            distTarget4ToZone = flatDistance(target4.transform.position, zone.transform.position);
            distTarget5ToZone = flatDistance(target5.transform.position, zone.transform.position);
            
            lastDistTarget1ToZone = rewardForPushingTargetFromZone(distTarget1ToZone, lastDistTarget1ToZone);
            lastDistTarget2ToZone = rewardForPushingTargetFromZone(distTarget2ToZone, lastDistTarget2ToZone);
            lastDistTarget3ToZone = rewardForPushingTargetFromZone(distTarget3ToZone, lastDistTarget3ToZone);
            lastDistTarget4ToZone = rewardForPushingTargetFromZone(distTarget4ToZone, lastDistTarget4ToZone);
            lastDistTarget5ToZone = rewardForPushingTargetFromZone(distTarget5ToZone, lastDistTarget5ToZone);

            rewardForApproachingTarget();
            //rewardText.text = part6.transform.position[1].ToString("000000");
            //Debug.Log(part6.transform.position[1]);
            if (part6.transform.position[1] - part1.transform.position[1] < 150)
            {
                AddReward(-5.0f);
                EndEpisode();
            }

            if (part6.transform.position[1] - part1.transform.position[1] < 200)
            {
                AddReward(-1.0f);
            }

            if (part5.transform.position[1] - part1.transform.position[1]< 100)
            {
                AddReward(-5.0f);
                EndEpisode();
            }

            if (part4.transform.position[1] - part1.transform.position[1]< 100)
            {
                AddReward(-5.0f);
                EndEpisode();
            }
            if (part3.transform.position[1] - part1.transform.position[1] < 100)
            {
                AddReward(-5.0f);
                EndEpisode();
            }

            if (target1.transform.position[1] - part1.transform.position[1] < 65)
            {
                AddReward(-5.0f);
                EndEpisode();
            }

            if (useTarget2 && target2.transform.position[1] - part1.transform.position[1] < 65)
            {
                AddReward(-5.0f);
                EndEpisode();
            }

            if (useTarget3 && target3.transform.position[1] - part1.transform.position[1] < 65)
            {
                AddReward(-5.0f);
                EndEpisode();
            }

            if (useTarget4 && target4.transform.position[1] - part1.transform.position[1] < 65)
            {
                AddReward(-5.0f);
                EndEpisode();
            }

            if (useTarget5 && target5.transform.position[1] - part1.transform.position[1] < 65)
            {
                AddReward(-5.0f);
                EndEpisode();
            }

            if (lastDistTarget1ToZone > zone_radius &&
                lastDistTarget2ToZone > zone_radius &&
                lastDistTarget3ToZone > zone_radius &&
                lastDistTarget4ToZone > zone_radius &&
                lastDistTarget5ToZone > zone_radius)
            {
                AddReward(5.0f);
                EndEpisode();
            }
            rewardText.text = this.GetCumulativeReward().ToString("000.0");
            doUpdate = false;
        }
    }

    public override float[] Heuristic()
    {
        var actionsOut = new float[4];
        if (Input.GetKey(KeyCode.D))
        {
            actionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            actionsOut[0] = 2;
        }
        else actionsOut[0] = 0;

        if (Input.GetKey(KeyCode.S))
        {
            actionsOut[2] = 2;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            actionsOut[2] = 1;
        }
        else actionsOut[2] = 0;


        if (Input.GetKey(KeyCode.U))
        {
            actionsOut[1] = 1;
        }
        else if (Input.GetKey(KeyCode.H))
        {
            actionsOut[1] = 2;
        }
        else actionsOut[1] = 0;


        if (Input.GetKey(KeyCode.G))
        {
            actionsOut[3] = 1;
        }
        else if (Input.GetKey(KeyCode.Y))
        {
            actionsOut[3] = 2;
        }
        else actionsOut[3] = 0;

        return actionsOut;
        //actionsOut[3] = Input.GetKey(KeyCode.Space) ? 1.0f : 0.0f;
    }

    private float rewardForPushingTargetFromZone(float distTargetToZone, float lastDistTargetToZone)
    {
        if (distTargetToZone < zone_radius)
        {
            if (distTargetToZone > lastDistTargetToZone + 1)
            {
                AddReward(posRewardPushTarget * distTargetToZone / zone_radius);
            }
            else if (distTargetToZone < lastDistTargetToZone - 1)
            {
                AddReward(negRewardPushTarget);
            }
            lastDistTargetToZone = distTargetToZone;
        }
        else if (lastDistTargetToZone < zone_radius)
        {
            AddReward(5.0f);
            lastDistTargetToZone = distTargetToZone;
        }
        return lastDistTargetToZone;
    }

    private void rewardForApproachingTarget()
    {
        //Debug.Log("distTarget1ToZone = " + distTarget1ToZone);
        //Debug.Log("distEndEffectorToTarget1 = " + distEndEffectorToTarget1);
        //Debug.Log("lastDistEndEffectorToTarget1 = " + lastDistEndEffectorToTarget1);
        if (distTarget1ToZone < zone_radius && distEndEffectorToTarget1 < lastDistEndEffectorToTarget1 - 1)
        {
            AddReward(posRewardApproachTarget);
        }
        else if (distTarget2ToZone < zone_radius && distEndEffectorToTarget2 < lastDistEndEffectorToTarget2 - 1)
        {
            AddReward(posRewardApproachTarget);
        }
        else if (distTarget3ToZone < zone_radius && distEndEffectorToTarget3 < lastDistEndEffectorToTarget3 - 1)
        {
            AddReward(posRewardApproachTarget);
        }
        else if (distTarget4ToZone < zone_radius && distEndEffectorToTarget4 < lastDistEndEffectorToTarget4 - 1)
        {
            AddReward(posRewardApproachTarget);
        }
        else if (distTarget5ToZone < zone_radius && distEndEffectorToTarget5 < lastDistEndEffectorToTarget5 - 1)
        {
            AddReward(posRewardApproachTarget);
        }
        else
        {
            //Debug.Log("Negative reward for not approaching target");
            AddReward(negRewardApproachTarget);
        }
        lastDistEndEffectorToTarget1 = distEndEffectorToTarget1;
        lastDistEndEffectorToTarget2 = distEndEffectorToTarget2;
        lastDistEndEffectorToTarget3 = distEndEffectorToTarget3;
        lastDistEndEffectorToTarget4 = distEndEffectorToTarget4;
        lastDistEndEffectorToTarget5 = distEndEffectorToTarget5;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float maxTargetRadius = 2500f;
        Vector3 target1FromPart1Position = new Vector3(
                                         target1.transform.position[0] - part1.transform.position[0],
                                         target1.transform.position[1] - part1.transform.position[1],
                                         target1.transform.position[2] - part1.transform.position[2]);
        sensor.AddObservation(target1FromPart1Position[0]/maxTargetRadius);
        sensor.AddObservation(target1FromPart1Position[1]/maxTargetRadius);
        sensor.AddObservation(target1FromPart1Position[2]/maxTargetRadius);

        Vector3 target2FromPart1Position = new Vector3(
                                         target2.transform.position[0] - part1.transform.position[0],
                                         target2.transform.position[1] - part1.transform.position[1],
                                         target2.transform.position[2] - part1.transform.position[2]);
        sensor.AddObservation(target2FromPart1Position[0]/maxTargetRadius);
        sensor.AddObservation(target2FromPart1Position[1]/maxTargetRadius);
        sensor.AddObservation(target2FromPart1Position[2]/maxTargetRadius);

        Vector3 target3FromPart1Position = new Vector3(
                                         target3.transform.position[0] - part1.transform.position[0],
                                         target3.transform.position[1] - part1.transform.position[1],
                                         target3.transform.position[2] - part1.transform.position[2]);
        sensor.AddObservation(target3FromPart1Position[0]/maxTargetRadius);
        sensor.AddObservation(target3FromPart1Position[1]/maxTargetRadius);
        sensor.AddObservation(target3FromPart1Position[2]/maxTargetRadius);

        Vector3 target4FromPart1Position = new Vector3(
                                         target4.transform.position[0] - part1.transform.position[0],
                                         target4.transform.position[1] - part1.transform.position[1],
                                         target4.transform.position[2] - part1.transform.position[2]);
        sensor.AddObservation(target4FromPart1Position[0]/maxTargetRadius);
        sensor.AddObservation(target4FromPart1Position[1]/maxTargetRadius);
        sensor.AddObservation(target4FromPart1Position[2]/maxTargetRadius);

        Vector3 target5FromPart1Position = new Vector3(
                                         target5.transform.position[0] - part1.transform.position[0],
                                         target5.transform.position[1] - part1.transform.position[1],
                                         target5.transform.position[2] - part1.transform.position[2]);
        sensor.AddObservation(target5FromPart1Position[0]/maxTargetRadius);
        sensor.AddObservation(target5FromPart1Position[1]/maxTargetRadius);
        sensor.AddObservation(target5FromPart1Position[2]/maxTargetRadius);

        Vector3 part4FromPart1Position = new Vector3(
                                       part4.transform.position[0] - part1.transform.position[0],
                                       part4.transform.position[1] - part1.transform.position[1],
                                       part4.transform.position[2] - part1.transform.position[2]);
        sensor.AddObservation(part4FromPart1Position[0]/maxTargetRadius);
        sensor.AddObservation(part4FromPart1Position[1]/maxTargetRadius);
        sensor.AddObservation(part4FromPart1Position[2]/maxTargetRadius);
        sensor.AddObservation(part61.transform.rotation[2] / 60);
        //sensor.AddObservation(part6.transform.rotation);
        // Vector3 zoneFromPart1Position = new Vector3(
        //                               zone.transform.position[0] - part1.transform.position[0],
        //                               zone.transform.position[1] - part1.transform.position[1],
        //                               zone.transform.position[2] - part1.transform.position[2]);
        // sensor.AddObservation(zoneFromPart1Position[0]/maxTargetRadius);
        // sensor.AddObservation(zoneFromPart1Position[1]/maxTargetRadius);
        // sensor.AddObservation(zoneFromPart1Position[2]/maxTargetRadius);
        // sensor.AddObservation(zone_radius/1000);
    }

    private float phi1 = 0;
    private float phi2 = 0.0f;
    private float phi3 = 0;
    private float phi4 = 0;
    private float phi5 = 0;
    private float phi6 = 0;
    private float phi61 = 0;
    private float phi62 = 0;
    private bool doGrab;

    public override void OnActionReceived(float[] vectorAction)
    {
        doUpdate = true;
        //Debug.Log("On action received");
        if (float.IsNaN(vectorAction[0])
            //float.IsNaN(vectorAction[1])||
            //float.IsNaN(vectorAction[2])
            )
        {
            //EndEpisode();
            Debug.Log("NAN");
            phi1 = normalizeAngle(++phi1);
            part1.transform.localRotation = Quaternion.Euler(0, phi1, 0);     
        }
        
        float x, z, y, rad_x, rad_y, rad_z, grab;

        x = part4.transform.position[0] - part1.transform.position[0];
        y = part4.transform.position[1] - part1.transform.position[1];
        z = part4.transform.position[2] - part1.transform.position[2];
        //Debug.Log("part4 x: " + x + " y: " + y + " z: " + z);
        var actionX = Mathf.FloorToInt(vectorAction[0]);
        var actionY = Mathf.FloorToInt(vectorAction[1]);
        var actionZ = Mathf.FloorToInt(vectorAction[2]);
        var actionGrab = Mathf.FloorToInt(vectorAction[3]);
        //Debug.Log(action);

        switch (actionX)
        {
            case 0:
                // do nothing
                break;
            case 1:
                // x+
                x += actionSpeed;
                break;
            case 2:
                // x-
                x -= actionSpeed;
                break;
            default:
                throw new ArgumentException("Invalid action value");
        }

        switch(actionY)
        {
            case 0:
                // do nothing
                break;
            case 1:
                // y+
                y += actionSpeed;
                break;
            case 2:
                if (y > 500)
                {
                // y-
                    y -= actionSpeed;
                }
                break;
            default:
                throw new ArgumentException("Invalid action value");
        }

        switch(actionZ)
        {
            case 0:
                // do nothing
                break;
            case 1:
                // z+
                z += actionSpeed;
                break;
            case 2:
                // z-
                z -= actionSpeed;
                break;
            default:
                throw new ArgumentException("Invalid action value");
        }

        switch(actionGrab)
        {
            case 0:
                // do nothing
                break;
            case 1:
                doGrab = true;
                break;
            case 2:
                doGrab = false;
                break;
            default:
                throw new ArgumentException("Invalid action value");
        }

        if (y < 500) y = 500;
        //Debug.Log(x + " " + y + " " + z);
        // x = vectorAction[0] * 3000;
        // y = vectorAction[1] * 3000;
        // z = vectorAction[2] * 3000;

        // grab = vectorAction[3];
        // bool doGrab = grab > 0.5;
        // Debug.Log("after action x: " + (int)x + " z: " + (int)y + " y: " + (int)z); 
        float[] coordinates = {x, z, y, 0, 0, (float)Math.PI};   
        float[] angles = reverseKinematics(coordinates);
        float[] backPos = calcPosition(angles);
        // Debug.Log("angles: "  + angles[0] + "//" + angles[1] + "//" + angles[2]  
        //                                  + "//" + angles[3] + "//" + angles[4] + "//" + angles[5]); 
        // Debug.Log("tpos: "  + backPos[0] + "//" + backPos[1] + "//" + backPos[2]  
        //                                 + "//" + backPos[3] + "//" + backPos[4] + "//" + backPos[5]); 
        // if (Mathf.Approximately(coordinates[0], backPos[0]) &&
        //     Mathf.Approximately(coordinates[1], backPos[1]) &&
        //     Mathf.Approximately(coordinates[2], backPos[2]))
        //     {
        //         Debug.Log("instant port");
        //         part1.transform.localRotation = Quaternion.Euler(0, angles[0], 0); 
        //         part2.transform.localRotation = Quaternion.Euler(0, 0, angles[1]); 
        //         part3.transform.localRotation = Quaternion.Euler(0, 0, angles[2]); 
        //         float[] newPos = {part4.transform.position[0] - part1.transform.position[0], 
        //                           part4.transform.position[2] - part1.transform.position[2], 
        //                           part4.transform.position[1] - part1.transform.position[1],
        //                           0, 0, (float) Math.PI};
        //         angles = reverseKinematics(newPos);
        //         part4.transform.localRotation = Quaternion.Euler(radToDegree(angles[3]), 0, 0);
        //         part5.transform.localRotation = Quaternion.Euler(0, 0, radToDegree(angles[4]));
        //         part6.transform.localRotation = Quaternion.Euler(radToDegree(angles[5]), 0, 0);
        //     }
        // else
        // {
            float target_phi1 = radToDegree(angles[0]);
            float target_phi2 = radToDegree(angles[1]);
            float target_phi3 = radToDegree(angles[2]);
            float target_phi4 = radToDegree(angles[3]);
            float target_phi5 = radToDegree(angles[4]);
            float target_phi6 = radToDegree(angles[5]);
            // check limits
            if (target_phi2 > part2_max)
            {
                target_phi2 = part2_max;
            }
            else if (target_phi2 < part2_min)
            {
                target_phi2 = part2_min;
            } 

            if (target_phi3 > part3_max)
            {
                target_phi3 = part3_max;
            } 
            else if (target_phi3 < part3_min)
            {
                target_phi3 = part3_min;
            }

            if (target_phi5 > part5_max) 
            {
                target_phi5 = part5_max;
            }
            else if (target_phi5 < part5_min)
            {
                target_phi5 = part5_min;
            } 

            float diff;
            /*******************phi 1**********************/
            diff = target_phi1 - phi1;
            if (diff > 0.1)
            {
                phi1 += speed;
            }
            else if (diff < -0.1)
            {
                phi1 -= speed;
            }
            phi1 = normalizeAngle(phi1);

            part1.transform.localRotation = Quaternion.Euler(0, phi1, 0); 
            /*******************phi 2**********************/
            diff = target_phi2 - phi2;
            if (diff > 0.1)
            {
                phi2 += speed;
            }
            else if ( diff < -0.1)
            {
                phi2 -= speed;
            }
            phi2 = normalizeAngle(phi2);
            part2.transform.localRotation = Quaternion.Euler(0, 0, phi2); 

            /*******************phi 3**********************/
            diff = target_phi3 - phi3;
            if (diff > 0.1)
            {
                phi3 += speed;
            }
            else if ( diff < -0.1)
            {
                phi3 -= speed;
            }
            phi3 = normalizeAngle(phi3);
            part3.transform.localRotation = Quaternion.Euler(0, 0, phi3); 
            float[] newPos = {part4.transform.position[0] - part1.transform.position[0], 
                            part4.transform.position[2] - part1.transform.position[2], 
                            part4.transform.position[1] - part1.transform.position[1],
                            0, 0, (float) Math.PI};
            angles = reverseKinematics(newPos);
            part4.transform.localRotation = Quaternion.Euler(radToDegree(angles[3]), 0, 0);
            part5.transform.localRotation = Quaternion.Euler(0, 0, radToDegree(angles[4]));
            part6.transform.localRotation = Quaternion.Euler(radToDegree(angles[5]), 0, 0);

            /****************end effector *************************/
            if (doGrab)
            {
                if (phi61 > end_min) phi61 -= speed*2;
                if (phi62 < -end_min) phi62 += speed*2;
            }
            else
            {
                if (phi61 < end_max) phi61 += speed*2;
                if (phi62 > -end_max) phi62 -= speed*2;
            }
            part61.transform.localRotation = Quaternion.Euler(0, 0, phi61);
            part62.transform.localRotation = Quaternion.Euler(0, 0, phi62);
        //}
        Update();

    }


    //private float distanceFromZone = 14f;

    public override void OnEpisodeBegin()
    {
        // Border update
        Vector3 pos = b1.transform.position;
        pos[1] = part1.transform.position[1];
        b1.transform.position = pos;

        pos = b2.transform.position;
        pos[1] = part1.transform.position[1];
        b2.transform.position = pos;

        pos = b3.transform.position;
        pos[1] = part1.transform.position[1];
        b3.transform.position = pos;

        pos = b4.transform.position;
        pos[1] = part1.transform.position[1];
        b4.transform.position = pos;

        pos = b5.transform.position;
        pos[1] = part1.transform.position[1];
        b5.transform.position = pos;

        pos = b6.transform.position;
        pos[1] = part1.transform.position[1];
        b6.transform.position = pos;

        pos = b7.transform.position;
        pos[1] = part1.transform.position[1];
        b7.transform.position = pos;

        pos = b8.transform.position;
        pos[1] = part1.transform.position[1];
        b8.transform.position = pos;


        collided = false;
        if (useCurricula)
        {
            float phase = (float) Academy.Instance.FloatProperties.GetPropertyWithDefault("phase", 1);
            //float phase = 5;
            switch (phase)
            {
                case 1:
                {
                    useTarget1 = true;
                    break;
                }
                case 2:
                {
                    useTarget2 = true;
                    break;
                }
                case 3:
                {
                    useTarget3 = true;
                    break;
                }
                case 4:
                {
                    useTarget4 = true;
                    break;
                }
                case 5:
                {
                    useTarget5 = true;
                    break;
                }
            }
        }

        Physics.gravity = new Vector3(0, -3000.0F, 0);
        // float offset1 = 200;
        // float offset2 = 200;
        // float offset3 = 200;
        // float offset4 = 200;
        // float offset5 = 200;

        float offset1 = 0;
        float offset2 = 0;
        float offset3 = 0;
        float offset4 = 0;
        float offset5 = 0;
        
        float margin = 110;

        float distanceFromZone = 500;
        if (useCurricula)
        {
            //distanceFromZone = (float) Academy.Instance.FloatProperties.GetPropertyWithDefault("distance_from_zone", 0);
        } 
        
        
        float whichTarget = UnityEngine.Random.value;
        if (useTarget2)
        {
            if (whichTarget < 0.2)
            {
                offset1 = 0;
            }
            else if (whichTarget < 0.4)
            {
                offset2 = 0;
            }
            else if (whichTarget < 0.6)
            {
                offset3 = 0;
            }
            else if (whichTarget < 0.8)
            {
                offset4 = 0;
            }
            else offset5 = 0;
        }
        else offset1 = 0;


        //11111111111111111111111111
        float rad = 2 * (float) Math.PI * UnityEngine.Random.value;
        float dist = zone_radius - margin - UnityEngine.Random.value * distanceFromZone + offset1;
        if (useTarget1)
        {

            Vector3 spawnTarget1 = zone.transform.position;


            //float dist = 500;
            spawnTarget1[0] += dist * (float)Math.Cos(rad);
            spawnTarget1[1] += 101;
            spawnTarget1[2] += dist * (float)Math.Sin(rad);
            target1.transform.position = spawnTarget1;
        }
        else
        {
            pos = zone.transform.position; 
            pos[0] += 1000;
            pos[1] += 100;
            pos[2] -= 300;
            target1.transform.position = pos;
        } 
        target1.GetComponent<Rigidbody>().velocity = Vector3.zero;
        target1.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        //22222222222222222222222222
        if (useTarget2) 
        { 
            rad = 2 * (float) Math.PI * UnityEngine.Random.value; 
            dist = zone_radius - margin - UnityEngine.Random.value * distanceFromZone + offset2; 
            pos = zone.transform.position; 
            pos[0] += dist * (float)Math.Cos(rad); 
            pos[1] += 101; 
            pos[2] += dist * (float)Math.Sin(rad); 
            target2.transform.position = pos; 
            target2.transform.localRotation = Quaternion.identity; 
 
            while (Vector3.Distance(target1.transform.position, target2.transform.position) < 220) 
            { 
                rad = 2 * (float) Math.PI * UnityEngine.Random.value; 
                dist = zone_radius - margin - UnityEngine.Random.value * distanceFromZone + offset2; 
                pos = zone.transform.position; 
                pos[0] += dist * (float)Math.Cos(rad); 
                pos[1] += 101; 
                pos[2] += dist * (float)Math.Sin(rad); 
                target2.transform.position = pos; 
                target2.transform.localRotation = Quaternion.identity; 
            } 
        } 
        else 
        {
            pos = zone.transform.position; 
            pos[0] += 1000;
            pos[1] += 100;
            pos[2] += 0;
            target2.transform.position = pos;
        } 
        target2.GetComponent<Rigidbody>().velocity = Vector3.zero; 
        target2.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; 

        //333333333333333333333333333333
        if (useTarget3) 
        { 
            rad = 2 * (float) Math.PI * UnityEngine.Random.value; 
            dist = zone_radius - margin - UnityEngine.Random.value * distanceFromZone + offset3; 
            pos = zone.transform.position; 
            pos[0] += dist * (float)Math.Cos(rad); 
            pos[1] += 101; 
            pos[2] += dist * (float)Math.Sin(rad); 
            target3.transform.position = pos; 
            target3.transform.localRotation = Quaternion.identity; 
 
            while (Vector3.Distance(target1.transform.position, target3.transform.position) < 220 ||
                   Vector3.Distance(target2.transform.position, target3.transform.position) < 220) 
            { 
                rad = 2 * (float) Math.PI * UnityEngine.Random.value; 
                dist = zone_radius - margin - UnityEngine.Random.value * distanceFromZone + offset3; 
                pos = zone.transform.position; 
                pos[0] += dist * (float)Math.Cos(rad); 
                pos[1] += 101; 
                pos[2] += dist * (float)Math.Sin(rad); 
                target3.transform.position = pos; 
                target3.transform.localRotation = Quaternion.identity; 
            } 
        } 
        else 
        { 
            pos = zone.transform.position; 
            pos[0] += 1000;
            pos[1] += 100;
            pos[2] += 300;
            target3.transform.position = pos;
        } 
        target3.GetComponent<Rigidbody>().velocity = Vector3.zero; 
        target3.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; 
        // //444444444444444444444444444444444
        if (useTarget4) 
        { 
            rad = 2 * (float) Math.PI * UnityEngine.Random.value; 
            dist = zone_radius - margin - UnityEngine.Random.value * distanceFromZone + offset4; 
            pos = zone.transform.position; 
            pos[0] += dist * (float)Math.Cos(rad); 
            pos[1] += 101; 
            pos[2] += dist * (float)Math.Sin(rad); 
            target4.transform.position = pos; 
            target4.transform.localRotation = Quaternion.identity; 
 
            while (Vector3.Distance(target1.transform.position, target4.transform.position) < 220 ||
                   Vector3.Distance(target2.transform.position, target4.transform.position) < 220 ||
                   Vector3.Distance(target3.transform.position, target4.transform.position) < 220) 
            { 
                rad = 2 * (float) Math.PI * UnityEngine.Random.value; 
                dist = zone_radius - margin - UnityEngine.Random.value * distanceFromZone + offset4; 
                pos = zone.transform.position; 
                pos[0] += dist * (float)Math.Cos(rad); 
                pos[1] += 101; 
                pos[2] += dist * (float)Math.Sin(rad); 
                target4.transform.position = pos; 
                target4.transform.localRotation = Quaternion.identity; 
            } 
        } 
        else 
        { 
            pos = zone.transform.position; 
            pos[0] += 1000;
            pos[1] += 100;
            pos[2] += 600;
            target4.transform.position = pos;
        } 
        target4.GetComponent<Rigidbody>().velocity = Vector3.zero; 
        target4.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; 

        //555555555555555555555555555555
        if (useTarget5) 
        { 
            rad = 2 * (float) Math.PI * UnityEngine.Random.value; 
            dist = zone_radius - margin - UnityEngine.Random.value * distanceFromZone + offset5; 
            pos = zone.transform.position; 
            pos[0] += dist * (float)Math.Cos(rad); 
            pos[1] += 101; 
            pos[2] += dist * (float)Math.Sin(rad); 
            target5.transform.position = pos; 
            target5.transform.localRotation = Quaternion.identity; 
 
            while (Vector3.Distance(target1.transform.position, target5.transform.position) < 220 ||
                   Vector3.Distance(target2.transform.position, target5.transform.position) < 220 ||
                   Vector3.Distance(target3.transform.position, target5.transform.position) < 220 ||
                   Vector3.Distance(target4.transform.position, target5.transform.position) < 220) 
            { 
                rad = 2 * (float) Math.PI * UnityEngine.Random.value; 
                dist = zone_radius - margin - UnityEngine.Random.value * distanceFromZone + offset5; 
                pos = zone.transform.position; 
                pos[0] += dist * (float)Math.Cos(rad); 
                pos[1] += 101; 
                pos[2] += dist * (float)Math.Sin(rad); 
                target5.transform.position = pos; 
                target5.transform.localRotation = Quaternion.identity; 
            } 
        } 
        else 
        { 
            pos = zone.transform.position; 
            pos[0] += 1000;
            pos[1] += 100;
            pos[2] += 900;
            target5.transform.position = pos;
        } 
        target5.GetComponent<Rigidbody>().velocity = Vector3.zero; 
        target5.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; 


        float onTop = 900;
        float tx, ty, tz;
        if (useTarget2)
        {
            if (whichTarget < 0.2)
            {
                tx = target1.transform.position[0];
                tz = target1.transform.position[2];
                ty = target1.transform.position[1] + onTop;
            }
            else if (whichTarget < 0.4)
            {
                tx = target2.transform.position[0];
                tz = target2.transform.position[2];
                ty = target2.transform.position[1] + onTop;
            }
            else if (whichTarget < 0.6)
            {
                tx = target3.transform.position[0];
                tz = target3.transform.position[2];
                ty = target3.transform.position[1] + onTop;
            }
            else if (whichTarget < 0.8)
            {
                tx = target4.transform.position[0];
                tz = target4.transform.position[2];
                ty = target4.transform.position[1] + onTop;
            }
            else
            {
                tx = target5.transform.position[0];
                tz = target5.transform.position[2];
                ty = target5.transform.position[1] + onTop;
            }
        }
        else
        {
            tx = target1.transform.position[0];
            tz = target1.transform.position[2];
            ty = target1.transform.position[1] + onTop;
        }



        float default_x = part1.transform.position[0] + 1400;
        float default_z = part1.transform.position[2];
        float default_y = part1.transform.position[1] + 1000;

        float dx = tx - default_x;
        float dy = ty - default_y;
        float dz = tz - default_z;

        float close_coeff = 0;
        if (useCurricula)
        {
            //close_coeff = (float) Academy.Instance.FloatProperties.GetPropertyWithDefault("close_coeff", 1.0f);
        }
        
        //Debug.Log(close_coeff);
        float[] tpos = {default_x + dx * close_coeff - part1.transform.position[0], 
                        default_z + dz * close_coeff - part1.transform.position[2], 
                        default_y + dy * close_coeff - part1.transform.position[1], 
                        0, 0, (float) Math.PI};
        // Debug.Log("tpos target*******************************: "  + tpos[0] + "//" + tpos[1] + "//" + tpos[2]  
        //         + "//" + tpos[3] + "//" + tpos[4] + "//" + tpos[5]); 
        float[] inverse = reverseKinematics(tpos);
        float[] backPos = calcPosition(inverse);
        // Debug.Log("angles: "  + inverse[0] + "//" + inverse[1] + "//" + inverse[2]  
        //                                  + "//" + inverse[3] + "//" + inverse[4] + "//" + inverse[5]); 
        // Debug.Log("tpos: "  + backPos[0] + "//" + backPos[1] + "//" + backPos[2]  
        //                                 + "//" + backPos[3] + "//" + backPos[4] + "//" + backPos[5]);
        phi1 = radToDegree(inverse[0]);
        phi2 = radToDegree(inverse[1]);
        phi3 = radToDegree(inverse[2]);
        phi4 = radToDegree(inverse[3]);
        phi5 = radToDegree(inverse[4]);
        phi6 = radToDegree(inverse[5]);
        phi61 = 0;
        phi62 = 0;
        float[] p4_pos = calcPosition(inverse);

        part1.transform.localRotation = Quaternion.Euler(0, phi1, 0);
        part2.transform.localRotation = Quaternion.Euler(0, 0, phi2);
        part3.transform.localRotation = Quaternion.Euler(0, 0, phi3);
        part4.transform.localRotation = Quaternion.Euler(phi4, 0, 0);
        part5.transform.localRotation = Quaternion.Euler(0, 0, phi5);
        part6.transform.localRotation = Quaternion.Euler(phi6, 0, 0);
        part61.transform.localRotation = Quaternion.Euler(0, 0, 0);
        part62.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public float normalizeAngle(float angle)
    {
        if (angle < -180) angle += 360;
        else if (angle > 180) angle -= 360;
        return angle;
    }

    public float radToDegree(float rad)
    {
        float result;
        result = rad * 180;
        result = result / (float) Math.PI;
        return result;
    }

    public float degreeToRad(float angle)
    {
        float result;
        result = angle * (float)Math.PI;
        result = result / 180;
        return result;
    }

    public float flatDistance(Vector3 p1, Vector3 p2)
    {
        return (float) Math.Sqrt(Math.Pow(p1[0] - p2[0], 2) + Math.Pow(p1[2] - p2[2], 2));
    }

    public float hypotenuse(float x1, float y1, float x2, float y2)
    {
        return  (float) Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
    }

    public Matrix4x4 DanHar(float a, float alpha, float d, float theta)
    {

        Matrix4x4 result;
        double c_t = Math.Cos(theta);
        double s_t = Math.Sin(theta);
        double c_a = Math.Cos(alpha);
        double s_a = Math.Sin(alpha);
        //first column
        result.m00 = (float) c_t;
        result.m10 = (float) s_t;
        result.m20 = 0;
        result.m30 = 0;
        // second column
        result.m01 = (float)(-s_t * c_a);
        result.m11 = (float) (c_t*c_a);
        result.m21 = (float) s_a;
        result.m31 = 0;
        //third col
        result.m02 = (float) (s_t*s_a);
        result.m12 = (float) (-c_t*s_a);
        result.m22 = (float)c_a;
        result.m32 = 0;
        // foulumn
        result.m03 = (float)(a*c_t);
        result.m13 = (float)(a*s_t);
        result.m23 = (float) d;
        result.m33 = 1;

        return result;
    }

    private void printMatrix(Matrix4x4 a, string name)
    {
        Debug.Log(name + " 1: " + a[0, 0] + "|" + a[0, 1] + "|" + a[0, 2] + "|" + a[0, 3]);
        Debug.Log(name + " 2: " + a[1, 0] + "|" + a[1, 1] + "|" + a[1, 2] + "|" + a[1, 3]);
        Debug.Log(name + " 3: " + a[2, 0] + "|" + a[2, 1] + "|" + a[2, 2] + "|" + a[2, 3]);
        Debug.Log(name + " 4: " + a[3, 0] + "|" + a[3, 1] + "|" + a[3, 2] + "|" + a[3, 3]);
    }
    public float[] reverseKinematics(float[] coordinates)
    {
        // Manipulator parameters
        int a2 = 1000;
        int d1 = 550;
        int d4 = 1240;
        int d6 = 320;

        double phi1, phi2, phi3, phi4, phi5, phi6;
        double x, y, z, rad_z, rad_y, rad_x;
        double bPow2, cPow2, a, b, c, alfa, beta;
        float  t1, t2, t3;
        x = coordinates[0];//real x
        y = -coordinates[1];//real z
        z = coordinates[2];//real y


        float[] angles;
        angles = new float[6];
        // phi1
        phi1 = Math.Atan2(y, x);
        angles[0] = (float)phi1;

        // phi3
        bPow2 = Math.Pow(z - d1, 2);
        cPow2 = Math.Pow(x, 2) + Math.Pow(y, 2);
        a = Math.Sqrt(bPow2 + cPow2);
        double cosphi3 = (bPow2 + cPow2 - Math.Pow(a2, 2) - Math.Pow(d4, 2)) / (2 * a2 * d4);
        phi3 = 0;
        if (Math.Abs(cosphi3 - 1) < 0.0000001)
        {
            phi3 = 0;
        } 
        else if (Math.Abs(cosphi3) < 0.0000001)
        {
            phi3 = Math.PI/2;
        }
        else
        {
            phi3 = Math.Atan2(Math.Sqrt(1 - Math.Pow(cosphi3, 2)), cosphi3);
        }
            
        if (phi3 > 0)
        {
            phi3 = -phi3;
        }

        b = Math.Sqrt(bPow2);
        c = Math.Sqrt(cPow2);
        alfa = Math.Atan2(b, c);

        double cosbeta = Math.Abs((Math.Pow(d4, 2) - Math.Pow(a, 2) - Math.Pow(a2, 2))/(2 * a * a2));
        beta = Math.Acos(cosbeta);
        if (z < d1)
        {
            phi2 = beta - alfa;
        }
        else if (phi3 < 0)
        {
            phi2 = alfa + beta;
        }
        else
        {
            phi2 = alfa - beta;
        }
        angles[1] = (float)phi2;
        angles[2] = (float)phi3;
        
        t1 = (float)phi1;
        t2 = (float)phi2;
        t3 = (float)phi3;

        Matrix4x4 A01 = DanHar(0,  (float)Math.PI/2, d1, t1);
        Matrix4x4 A12 = DanHar(a2,    0,  0, t2);
        Matrix4x4 A23 = DanHar(0,  (float)Math.PI/2,  0, t3 + (float)Math.PI/2);

        Matrix4x4 A02;
        Matrix4x4 A03;

        A02 = A01*A12;
        A03 = A02*A23;
        A03[0, 3] = 0;
        A03[1, 3] = 0;
        A03[2, 3] = 0;
        //printMatrix(A03, "A03");


        Matrix4x4 A03Transposed;
        A03Transposed = A03.transpose;
        //printMatrix(A03Transposed, "A03Transposed");
        Matrix4x4 rotMatrix;
        Matrix4x4 rotX;
        Matrix4x4 rotY;
        Matrix4x4 rotZ;

        rad_z = coordinates[3];
        rad_y = coordinates[4];
        rad_x = coordinates[5];

        //X
        rotX.m00 = 1;
        rotX.m10 = 0;
        rotX.m20 = 0;
        rotX.m30 = 0;

        rotX.m01 = 0;
        rotX.m11 = (float) Math.Cos(rad_x);
        rotX.m21 = (float) Math.Sin(rad_x);
        rotX.m31 = 0;

        rotX.m02 = 0;
        rotX.m12 = - (float) Math.Sin(rad_x);
        rotX.m22 = (float) Math.Cos(rad_x);
        rotX.m32 = 0;

        rotX.m03 = 0;
        rotX.m13 = 0;
        rotX.m23 = 0;
        rotX.m33 = 1;

        //Y
        rotY.m00 = (float) Math.Cos(rad_y);
        rotY.m10 = 0;
        rotY.m20 = - (float)Math.Sin(rad_y);
        rotY.m30 = 0;

        rotY.m01 = 0;
        rotY.m11 = 1;
        rotY.m21 = 0;
        rotY.m31 = 0;

        rotY.m02 = (float) Math.Sin(rad_y);
        rotY.m12 = 0;
        rotY.m22 = (float) Math.Cos(rad_y);
        rotY.m32 = 0;

        rotY.m03 = 0;
        rotY.m13 = 0;
        rotY.m23 = 0;
        rotY.m33 = 1;

        //Z
        rotZ.m00 = (float) Math.Cos(rad_z);
        rotZ.m10 = (float) Math.Sin(rad_z);
        rotZ.m20 = 0;
        rotZ.m30 = 0;

        rotZ.m01 = -(float)Math.Sin(rad_z);
        rotZ.m11 = (float) Math.Cos(rad_z);
        rotZ.m21 = 0;
        rotZ.m31 = 0;

        rotZ.m02 = 0;
        rotZ.m12 = 0;
        rotZ.m22 = 1;
        rotZ.m32 = 0;

        rotZ.m03 = 0;
        rotZ.m13 = 0;
        rotZ.m23 = 0;
        rotZ.m33 = 1;

        rotMatrix = rotZ * rotY * rotX;

        Matrix4x4 A36;
        A36 = A03Transposed * rotMatrix;

        if (A36[2, 2] == 0)
        {
            phi5 = 0.001;
        }
        else if (A36[2, 2] == -1)
        {
            phi5 = Math.PI - 0.001;
        }
        else
        {
            phi5 = Math.Atan2(Math.Sqrt(1 - Math.Pow(A36[2, 2], 2)), A36[2, 2]);
        }
        phi4 = Math.Atan2(A36[1, 2], A36[0, 2]);
        phi6 = Math.Atan2(A36[2, 1], -A36[2, 0]);

        angles[3] = (float)phi4;
        angles[4] = (float)phi5;
        angles[5] = -(float)phi6;

        return angles;
    }

    public float[] calcPosition(float[] angles)
    {
        float[] coordinates;
        coordinates = new float[6];
        float t1, t2, t3, t4, t5, t6;
        t1 = angles[0];
        t2 = angles[1];
        t3 = angles[2];
        t4 = angles[3];
        t5 = angles[4];
        t6 = angles[5];
        float d1 = 550;
        float a2 = 1000;
        float d4 = 1240;
        float d6 = 320;

        
        Matrix4x4 A02;
        Matrix4x4 A03;
        Matrix4x4 A04;
        Matrix4x4 A05;
        Matrix4x4 A06;

        Matrix4x4 A01;
        Matrix4x4 A12;
        Matrix4x4 A23;
        Matrix4x4 A34;
        Matrix4x4 A45;
        Matrix4x4 A56;
        float pi = (float) Math.PI;
        A01 = DanHar(0,  pi/2, d1, t1);
        A12 = DanHar(a2,    0,  0, t2);
        A23 = DanHar(0,  pi/2,  0, t3 + pi/2);
        A34 = DanHar(0, -pi/2, d4, t4);
        A45 = DanHar(0,  pi/2,  0, t5);
        A56 = DanHar(0,     0, d6, t6); 

        A02 = A01 * A12;
        A03 = A02 * A23;
        A04 = A03 * A34;
        A05 = A04 * A45;
        A06 = A05 * A56;

        Matrix4x4 R1;
        Matrix4x4 R;
        Matrix4x4 res;
        R.m00 = 0;
        R.m10 = 0;
        R.m20 = 0;
        R.m30 = 0;
        ///////////////
        R.m01 = 0;
        R.m11 = 0;
        R.m21 = 0;
        R.m31 = 0;
        /////////////
        R.m02 = 0;
        R.m12 = 0;
        R.m22 = 0;
        R.m32 = 0;
        //////////////
        R.m03 = 0;
        R.m13 = 0;
        R.m23 = 0;
        R.m33 = 1;

        R1 = A06;
        res = A04 * R;
        coordinates[0] =  res.m03;
        coordinates[1] = -res.m13;
        coordinates[2] =  res.m23;

        float sy = (float)Math.Sqrt(Math.Pow(R1.m00, 2) + Math.Pow(R1.m10, 2));

        bool singular = sy < 0.000001;

        if (singular)
        {
            //z
            coordinates[3] = 0;
            //y
            coordinates[4] = (float) Math.Atan2(-R1.m20, sy);
            //x
            coordinates[5] = (float) Math.Atan2(-R1.m12, R1.m11);
        }
        else
        {
            //z
            coordinates[3] = (float) Math.Atan2(R1.m10, R1.m00);
            //y
            coordinates[4] = (float) Math.Atan2(-R1.m20, sy);
            //x
            coordinates[5] = (float) Math.Atan2(R1.m21, R1.m22);
        }
        return coordinates;

    }

}
