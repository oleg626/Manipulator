using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;
using TMPro;
using System;
using System.Threading;

public class ManipulatorAgent : Agent
{

    public GameObject part1, part2, part3, part4, part5, part6, part61, part62, target1, target2, target3, target4, target5, zone, cube;
    public TextMeshPro rewardText;
    public float speed;
    public float part2_min =  0;
    public float part2_max =  90;
    public float part3_min = -110;
    public float part3_max =  110;
    public float part5_min = -100;
    public float part5_max =  100;
    public float end_min = 1;
    public float end_max =  60;
    public float zone_radius = 750;

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

    private void Update()
    {
        // float discretization = (float) Math.PI / 3;
        // float[] position = new float[6];
        // float[] inverse = new float[6];
        // float[] pos = new float[6];
        // //float[] angles = new float[6];
        // float pi = (float) Math.PI;
        // float d1, d2, d3, d4, d5, d6;
        // Vector3 p4 = part4.transform.position;
        //Debug.Log("x: " + p4[0] + "//y:" + p4[1] + "//z:" + p4[2]);
        // for (float i1 = - pi/2; i1 < pi/2; i1 += discretization)
        // {
        //     for (float i2 = 0; i2 < pi/2; i2 += discretization)
        //     {
        //         for (float i3 = - pi/2; i3 < pi/2; i3 += discretization)
        //         {
        //             for (float i4 = - pi/3; i4 < pi/3; i4 += discretization)
        //             {
        //                 for (float i5 = - pi/3; i5 < pi/3; i5 += discretization)
        //                 {
        //                     for (float i6 = - pi/3; i6 < pi/3; i6 += discretization)
        //                     {
        //                         // float i1 = -pi/2;
        //                         // float i2 = 0;
        //                         // float i3 = -pi/2;
        //                         // float i4 = -pi/3;
        //                         // float i5 = -pi/3;
        //                         // float i6 = -pi/3;
        //                         float[] angles = {i1, i2, i3, i4, i5, i6};
        //                         position = calcPosition(angles);
        //                         inverse = reverseKinematics(position);
        //                         pos = calcPosition(inverse);
        //                         d1 = Math.Abs(position[0] - pos[0]);
        //                         d2 = Math.Abs(position[1] - pos[1]);
        //                         d3 = Math.Abs(position[2] - pos[2]);
        //                         d4 = Math.Abs(position[3] - pos[3]);
        //                         d5 = Math.Abs(position[4] - pos[4]);
        //                         d6 = Math.Abs(position[5] - pos[5]);

        //                         if (d1 > 0.01 || d2 > 0.01 || d3 > 0.01 || d4 > 0.05 || d5 > 0.05 || d6 > 0.05)
        //                         {
        //                             Debug.Log("ERROR");
        //                             Debug.Log("input values: " + i1 + "// " + i2 + "//" + i3 + "//" + i4 + "//" + i5 + "//" + i6);
        //                             Debug.Log("Inverse: " + inverse[0] + "//" + inverse[1] + "//" + inverse[2] 
        //                                 + "//" + inverse[3] + "//" + inverse[4] + "//" + inverse[5]);
        //                             Debug.Log("position: "  + position[0] + "//" + position[1] + "//" + position[2] 
        //                                 + "//" + position[3] + "//" + position[4] + "//" + position[5]);
        //                             Debug.Log("Second position: " + pos[0] + "//" + pos[1] + "//" + pos[2] 
        //                                 + "//" + pos[3] + "//" + pos[4] + "//" + pos[5]);
        //                             Debug.Log("Error value: " + d1 + "//" + d2 + "//" + d3 + "//" + d4 + "//" + d5 + "//" + d6);
        //                         }
        //                         //Thread.Sleep(5000);
        //                     }
        //                 }
        //             }
        //         }
        //     }
        // }
        if (this.GetCumulativeReward() < - 500) EndEpisode();
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

        if (lastDistTarget1ToZone > zone_radius &&
            lastDistTarget2ToZone > zone_radius &&
            lastDistTarget3ToZone > zone_radius &&
            lastDistTarget4ToZone > zone_radius &&
            lastDistTarget5ToZone > zone_radius)
        {
            //Debug.Log("Episode end");
            AddReward(100f);
            EndEpisode();
        }

        rewardText.text = this.GetCumulativeReward().ToString("000000");
    }

    public override float[] Heuristic()
    {
        var action = new float[7];
        return action;
    }
    private float rewardForPushingTargetFromZone(float distTargetToZone, float lastDistTargetToZone)
    {
        if (distTargetToZone < zone_radius)
        {
            if (distTargetToZone > lastDistTargetToZone)
            {
                AddReward(0.2f);
            }
            else
            {
                AddReward(-0.2f);
            }
            lastDistTargetToZone = distTargetToZone;
        }
        else if (lastDistTargetToZone < zone_radius)
        {
            AddReward(100f);
            lastDistTargetToZone = distTargetToZone;
        }
        return lastDistTargetToZone;
    }

    private void rewardForApproachingTarget()
    {
        if (distTarget1ToZone < zone_radius)
        {
            if (distEndEffectorToTarget1 < lastDistEndEffectorToTarget1)
            {
                AddReward(0.1f);
            }
            else
            {
                AddReward(-0.1f);
            }
            lastDistEndEffectorToTarget1 = distEndEffectorToTarget1;
        }
        //2222222222222222222222222222222222
        // if (distTarget2ToZone < zone_radius)
        // {
        //     if (distEndEffectorToTarget2 < lastDistEndEffectorToTarget2)
        //     {
        //         AddReward(0.1f);
        //     }
        //     else
        //     {
        //         AddReward(-0.1f);
        //     }
        //     lastDistEndEffectorToTarget2 = distEndEffectorToTarget2;
        // }
        // //3333333333333333333333333333333333
        // if (distTarget3ToZone < zone_radius)
        // {
        //     if (distEndEffectorToTarget3 < lastDistEndEffectorToTarget3)
        //     {
        //         AddReward(0.1f);
        //     }
        //     else
        //     {
        //         AddReward(-0.1f);
        //     }
        //     lastDistEndEffectorToTarget3 = distEndEffectorToTarget3;
        // }
        // //444444444444444444444444444444444
        // if (distTarget4ToZone < zone_radius)
        // {
        //     if (distEndEffectorToTarget4 < lastDistEndEffectorToTarget4)
        //     {
        //         AddReward(0.1f);
        //     }
        //     else
        //     {
        //         AddReward(-0.1f);
        //     }
        //     lastDistEndEffectorToTarget4 = distEndEffectorToTarget4;
        // }
        // //5555555555555555555555555555555
        // if (distTarget5ToZone < zone_radius)
        // {
        //     if (distEndEffectorToTarget5 < lastDistEndEffectorToTarget5)
        //     {
        //         AddReward(0.1f);
        //     }
        //     else
        //     {
        //         AddReward(-0.1f);
        //     }
        //     lastDistEndEffectorToTarget5 = distEndEffectorToTarget5;
        // }


        //Debug.Log(part6.transform.position[1]);
        if (part6.transform.position[1] < 150)
        {
            //Debug.Log("part 6 low");
            //Debug.Log(part6.transform.position[1]);
            AddReward(-200f);
            EndEpisode();
        }
        if (part5.transform.position[1] < 100)
        {
            //Debug.Log("part 5 low");
            AddReward(-200f);
            EndEpisode();
        }

        if (part4.transform.position[1] < 100)
        {
            //Debug.Log("part 4 low");
            AddReward(-200f);
            EndEpisode();
        }
        if (part3.transform.position[1] < 100)
        {
            //Debug.Log("part 3 low");
            AddReward(-200f);
            EndEpisode();
        }

        if (target1.transform.position[1] < 90)
        {
            //Debug.Log("part 3 low");
            AddReward(-200f);
            EndEpisode();
        }
        
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target1.transform.position);
        // sensor.AddObservation(target2.transform.position);
        // sensor.AddObservation(target3.transform.position);
        // sensor.AddObservation(target4.transform.position);
        // sensor.AddObservation(target5.transform.position);
        sensor.AddObservation(part6.transform.position);
        sensor.AddObservation(part6.transform.rotation);
        sensor.AddObservation(zone.transform.position);
        sensor.AddObservation(zone_radius);
    }

    private float phi1 = 0;
    private float phi2 = 0;
    private float phi3 = 0;
    private float phi4 = 0;
    private float phi5 = 0;
    private float phi6 = 0;
    private float phi61 = 0;
    private float phi62 = 0;

    public override void OnActionReceived(float[] vectorAction)
    {
        if (float.IsNaN(vectorAction[0])||
            float.IsNaN(vectorAction[1])||
            float.IsNaN(vectorAction[2])
            //float.IsNaN(vectorAction[3])||
            //float.IsNaN(vectorAction[4])||
            //float.IsNaN(vectorAction[5])
            )
        {
            //EndEpisode();
            phi1 = normalizeAngle(++phi1);
            part1.transform.localRotation = Quaternion.Euler(0, phi1, 0);     
        }
        // float[] tpos = {target1.transform.position[0], target1.transform.position[2], 900, 0, 0, (float) Math.PI};
        // float[] inverse = reverseKinematics(tpos);
        // Debug.Log("tpos: "  + tpos[0] + "//" + tpos[1] + "//" + tpos[2] 
        //                                 + "//" + tpos[3] + "//" + tpos[4] + "//" + tpos[5]);
        // Debug.Log("inverse: "  + inverse[0] + "//" + inverse[1] + "//" + inverse[2] 
        //                                  + "//" + inverse[3] + "//" + inverse[4] + "//" + inverse[5]);
        // phi1 = inverse[0];
        // phi2 = inverse[1];
        // phi3 = inverse[2];
        // phi4 = inverse[3];
        // phi5 = inverse[4];
        // phi6 = inverse[5];
        // phi61 = 0;
        // phi62 = 0;
        // float[] p4_pos = calcPosition(inverse);
        // Debug.Log("position: "  + p4_pos[0] + "//" + p4_pos[1] + "//" + p4_pos[2] 
        //                                  + "//" + p4_pos[3] + "//" + p4_pos[4] + "//" + p4_pos[5]);
        // //Debug.Log("Episode Begin");
        // part1.transform.localRotation = Quaternion.Euler(0, radToDegree(phi1), 0);
        // part2.transform.localRotation = Quaternion.Euler(0, 0, radToDegree(phi2));
        // part3.transform.localRotation = Quaternion.Euler(0, 0, radToDegree(phi3));
        // part4.transform.localRotation = Quaternion.Euler(radToDegree(phi4), 0, 0);
        // part5.transform.localRotation = Quaternion.Euler(0, 0, radToDegree(phi5));
        // part6.transform.localRotation = Quaternion.Euler(radToDegree(phi6), 0, 0);
        // part61.transform.localRotation = Quaternion.Euler(0, 0, 0);
        // part62.transform.localRotation = Quaternion.Euler(0, 0, 0);
        //Debug.Log("0: " + vectorAction[0] + "//1:" + vectorAction[1] + "//2:" + vectorAction[2] + "//3:" + vectorAction[3] + "//4:" + vectorAction[4] + "//5:" + vectorAction[5]);
        float x, z, y, rad_x, rad_y, rad_z, grab;
        x = vectorAction[0] * 3000;
        y = vectorAction[1] * 3000;
        z = vectorAction[2] * 3000;
        //rad_z = vectorAction[3] * (float)(2 * Math.PI);
        //rad_y = vectorAction[4] * (float)(2 * Math.PI);
        //rad_x = vectorAction[5] * (float)(2 * Math.PI);

        //Debug.Log("x: " + (int)x + " z: " + (int)y + " y: " + (int)z + " rz: " + Math.Round(rad_z, 2) + " ry: " + Math.Round(rad_y, 2) + " rx: " + Math.Round(rad_x, 2));
        grab = vectorAction[3];
        bool doGrab = grab > 0.5;

        float[] coordinates = {x, z, y, 0, 0, (float)Math.PI};   
        float[] angles = reverseKinematics(coordinates);



        // Debug.Log("tpos: "  + tpos[0] + "//" + tpos[1] + "//" + tpos[2] 
        //                                 + "//" + tpos[3] + "//" + tpos[4] + "//" + tpos[5]);
        // Debug.Log("angles: "  + angles[0] + "//" + angles[1] + "//" + angles[2] 
        //                                  + "//" + angles[3] + "//" + angles[4] + "//" + angles[5]);
        // Debug.Log("inverse: " + angles[0] + "//1:" + angles[1] + "//2:" + angles[2] + "//3:" + angles[3] + "//4:" + angles[4] + "//5:" + angles[5]);
        //float[] p4pos = calcPosition(angles);
        //Debug.Log("pos x: " + p4pos[0] + "//z:" + p4pos[1] + "//y:" + p4pos[2] + "//rz:" + p4pos[3] + "//ry:" + p4pos[4] + "//rx:" + p4pos[5]);
        float target_phi1 = radToDegree(angles[0]);
        float target_phi2 = radToDegree(angles[1]);
        float target_phi3 = radToDegree(angles[2]);
        float target_phi4 = radToDegree(angles[3]);
        float target_phi5 = radToDegree(angles[4]);
        float target_phi6 = radToDegree(angles[5]);
        // check limits
        if (target_phi2 > part2_max)
        {
            //AddReward(-10f);
            target_phi2 = part2_max;
        }
        else if (target_phi2 < part2_min)
        {
            //AddReward(-10f);
            target_phi2 = part2_min;
        } 

        if (target_phi3 > part3_max)
        {
            //AddReward(-10f);
            target_phi3 = part3_max;
        } 
        else if (target_phi3 < part3_min)
        {
            //AddReward(-10f);
            target_phi3 = part3_min;
        }

        if (target_phi5 > part5_max) 
        {
            //AddReward(-10f);
            target_phi5 = part5_max;
        }
        else if (target_phi5 < part5_min)
        {
            //AddReward(-10f);  
            target_phi5 = part5_min;
        } 

        float diff;
        /*******************phi 1**********************/
        diff = target_phi1 - phi1;
        if (diff > 1)
        {
            phi1 += speed;
        }
        else if (diff < -1)
        {
            phi1 -= speed;
        }
        phi1 = normalizeAngle(phi1);

        part1.transform.localRotation = Quaternion.Euler(0, phi1, 0); 

        /*******************phi 2**********************/
        diff = target_phi2 - phi2;
        //Debug.Log("phi2 before:" + phi2);
        if (diff > 1)
        {
            phi2 += speed;
        }
        else if ( diff < -1)
        {
            phi2 -= speed;
        }
        phi2 = normalizeAngle(phi2);

        part2.transform.localRotation = Quaternion.Euler(0, 0, phi2); 

        /*******************phi 3**********************/
        diff = target_phi3 - phi3;
        if (diff > 1)
        {
            phi3 += speed;
        }
        else if ( diff < -1)
        {
            phi3 -= speed;
        }
        phi3 = normalizeAngle(phi3);
        part3.transform.localRotation = Quaternion.Euler(0, 0, phi3); 

        /*******************phi 4**********************/
        diff = target_phi4 - phi4;
        if (diff > 1)
        {
            phi4 += speed * 2;
        }
        else if ( diff < -1)
        {
            phi4 -= speed * 2;
        }
        else if (diff > 0 && diff < 1)
        {
            phi4 += speed/8;
        }
        else
        {
            phi4 -= speed/8;
        }
        phi4 = normalizeAngle(phi4);
        part4.transform.localRotation = Quaternion.Euler(phi4, 0, 0);

        /*******************phi 5**********************/
        diff = target_phi5 - phi5;
        if (diff > 1)
        {
            phi5 += speed * 2;
        }
        else if ( diff < -1)
        {
            phi5 -= speed * 2;
        }
        else if (diff > 0 && diff < 1)
        {
            phi5 += speed/8;
        }
        else
        {
            phi5 -= speed/8;
        }
        phi5 = normalizeAngle(phi5);
        part5.transform.localRotation = Quaternion.Euler(0, 0, phi5);

        /*******************phi 6**********************/
        diff = target_phi6 - phi6;
        if (diff > 1)
        {
            phi6 += speed * 2;
        }
        else if ( diff < -1)
        {
            phi6 -= speed * 2;
        }
        else if (diff > 0 && diff < 1)
        {
            phi6 += speed/8;
        }
        else
        {
            phi6 -= speed/8;
        }
        phi6 = normalizeAngle(phi6);
        part6.transform.localRotation = Quaternion.Euler(phi6, 0, 0);

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

        Update();

    }

    private bool useTarget1 = true;
    private bool useTarget2 = false;
    private bool useTarget3 = false;
    private bool useTarget4 = false;
    private bool useTarget5 = false;
    private float distanceFromZone = 200f;

    public override void OnEpisodeBegin()
    {

        //11111111111111111111111111
        // if (useTarget1)
        // {
        //     Vector3 pos = zone.transform.position;
        //     pos[0] += UnityEngine.Random.value * distanceFromZone * 2.0f - distanceFromZone;
        //     pos[1] += 102;
        //     pos[2] += UnityEngine.Random.value * distanceFromZone * 2.0f - distanceFromZone;
        //     target1.transform.position = new Vector3(pos[0], pos[1], pos[2]);
        //     target1.transform.localRotation = Quaternion.identity;
        // }
        // else
        // {
        target1.transform.position = new Vector3(1500, 102, 200);
        //}
        target1.GetComponent<Rigidbody>().velocity = Vector3.zero;
        target1.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        //22222222222222222222222222
        // if (useTarget2)
        // {
        //     Vector3 pos = zone.transform.position;
        //     pos[0] += UnityEngine.Random.value * distanceFromZone * 2.0f - distanceFromZone;
        //     pos[1] += 102;
        //     pos[2] += UnityEngine.Random.value * distanceFromZone * 2.0f - distanceFromZone;
        //     target2.transform.position = pos;
        //     target2.transform.localRotation = Quaternion.identity;
        // }
        // else
        // {
        //     target2.transform.position = new Vector3(3300, 102, 0);
        // }
        // target2.GetComponent<Rigidbody>().velocity = Vector3.zero;
        // target2.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        // //333333333333333333333333333333
        // if (useTarget3)
        // {
        //     Vector3 pos = zone.transform.position;
        //     pos[0] += UnityEngine.Random.Range(-distanceFromZone, distanceFromZone);
        //     pos[1] += 102;
        //     pos[0] += UnityEngine.Random.Range(-distanceFromZone, distanceFromZone);
        //     target3.transform.position = pos;
        // }
        // else
        // {
        //     target3.transform.position = new Vector3(3600, 102, 0);
        // }
        // target3.GetComponent<Rigidbody>().velocity = Vector3.zero;
        // target3.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        // //444444444444444444444444444444444
        // if (useTarget4)
        // {
        //     Vector3 pos = zone.transform.position;
        //     pos[0] += UnityEngine.Random.Range(-distanceFromZone, distanceFromZone);
        //     pos[1] += 102;
        //     pos[0] += UnityEngine.Random.Range(-distanceFromZone, distanceFromZone);
        //     target4.transform.position = pos;
        // }
        // else
        // {
        //     target4.transform.position = new Vector3(3900, 102, 0);
        // }
        // target4.GetComponent<Rigidbody>().velocity = Vector3.zero;
        // target4.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        // //555555555555555555555555555555
        // if (useTarget5)
        // {
        //     Vector3 pos = zone.transform.position;
        //     pos[0] += UnityEngine.Random.Range(-distanceFromZone, distanceFromZone);
        //     pos[1] += 102;
        //     pos[0] += UnityEngine.Random.Range(-distanceFromZone, distanceFromZone);
        //     target5.transform.position = pos;
        // }
        // else
        // {
        //     target5.transform.position = new Vector3(4200, 102, 0);
        // }
        // target5.GetComponent<Rigidbody>().velocity = Vector3.zero;  
        // target5.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;


        float[] tpos = {target1.transform.position[0], target1.transform.position[2], 750, 0, 0, (float) Math.PI};
        float[] inverse = reverseKinematics(tpos);
        // Debug.Log("tpos: "  + tpos[0] + "//" + tpos[1] + "//" + tpos[2] 
        //                                 + "//" + tpos[3] + "//" + tpos[4] + "//" + tpos[5]);
        phi1 = radToDegree(inverse[0]);
        phi2 = radToDegree(inverse[1]);
        phi3 = radToDegree(inverse[2]);
        phi4 = radToDegree(inverse[3]);
        phi5 = radToDegree(inverse[4]);
        phi6 = radToDegree(inverse[5]);
        phi61 = 0;
        phi62 = 0;
        //float[] p6_pos = calcPosition(inverse);
        // Debug.Log("inverse: "  + inverse[0] + "//" + inverse[1] + "//" + inverse[2] 
        //                                  + "//" + inverse[3] + "//" + inverse[4] + "//" + inverse[5]);
        // Debug.Log("position: "  + p6_pos[0] + "//" + p6_pos[1] + "//" + p6_pos[2] 
        //                                 + "//" + p6_pos[3] + "//" + p6_pos[4] + "//" + p6_pos[5]);
        ////Debug.Log("Episode Begin");
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
        // A = [c_t   -s_t*c_a   s_t*s_a   a*c_t;
        //      s_t    c_t*c_a  -c_t*s_a   a*s_t;
        //     0       s_a        c_a       d;
        //     0        0          0        1];

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
        else if (cosphi3 < 0.0000001)
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
        //printMatrix(rotMatrix, "rotMatrix");
        Matrix4x4 A36;
        A36 = A03Transposed * rotMatrix;
        //printMatrix(A36, "A36");
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
