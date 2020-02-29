using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class PlayerController
{
    /*
     * Simplify fields
     */
    PlayerView View;
    PlayerModel Model;
    PlayerController Controller;

    public delegate void OnAir(PLAYER_STATE state);
    public OnAir onAir;
    public delegate void OnJump();
    public OnJump onJump;

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        View = PlayerView.Instance;
        Model = PlayerView.Instance.Model;
        Controller = PlayerView.Instance.Controller;

        Model.rb = View.GetComponent<Rigidbody>();
        Model.coll = View.GetComponent<Collider>();

        Model.movement = Vector3.zero;

        Model.groundNormal = Vector3.zero;
        Model.touchingDynamic = false;
        Model.groundedLastFrame = false;

        Model.collisions = new List<GameObject>();
        Model.contactPoints = new Dictionary<int, ContactPoint[]>();

        // do our calculations so we don't have to do them every frame
        CapsuleCollider collider = (CapsuleCollider)Model.coll;
        //Model.halfPlayerHeight = collider.sharedMesh.bounds.size.y * 0.5f;
        Model.halfPlayerHeight = collider.height * 0.5f;
        //Model.bottomCapsuleSphereOrigin = collider.sharedMesh.bounds.min.y;
        Model.bottomCapsuleSphereOrigin = Model.halfPlayerHeight - collider.radius;
        //Model.capsuleRadius = collider.sharedMesh.bounds.extents.x;
        Model.capsuleRadius = collider.radius;

        PhysicMaterial controllerMat = new PhysicMaterial();
        controllerMat.bounciness = 0.0f;
        controllerMat.dynamicFriction = 0.0f;
        controllerMat.staticFriction = 0.0f;
        controllerMat.bounceCombine = PhysicMaterialCombine.Minimum;
        controllerMat.frictionCombine = PhysicMaterialCombine.Minimum;
        collider.material = controllerMat;

        // just in case this wasn't set in the inspector
        Model.rb.freezeRotation = true;

        // quartenion initialize
        Model.targetPivotRotation = Quaternion.identity;
    }

    public void Start()
    {
        //Model.cameraPivot = Model.go.transform;
        Model.StepQueue = new List<string>(20);
        Model.go = GameObject.FindGameObjectWithTag("Player");
        Model.playerState = PLAYER_STATE.None;
        Model.locomotionState = LOCOMOTION_STATE.None;
    }

    public void FixedUpdate()
    {
        UpdateVelocity();
        RaycastHit hit;
        SetState(PLAYER_STATE.Air);
        Model.groundNormal = Vector3.zero;

        // check slopes
        foreach (ContactPoint[] contacts in Model.contactPoints.Values)
        {
            for (int i = 0; i < contacts.Length; i++)
                if (contacts[i].point.y <= Model.rb.position.y - Model.bottomCapsuleSphereOrigin && Physics.Raycast(contacts[i].point + Vector3.up, Vector3.down, out hit, 1.1f, ~0) && Vector3.Angle(hit.normal, Vector3.up) <= PlayerStats.MaximumSlope)
                {
                    Model.groundNormal += hit.normal;
                    SetState(PLAYER_STATE.Grounded);
                }
        }

        if (Model.playerState == PLAYER_STATE.Grounded)
        {
            // average the summed normals
            Model.groundNormal.Normalize();
        }

        // get player input
        Model.inputX = Input.GetAxis("Horizontal");
        Model.inputY = Input.GetAxis("Vertical");

        // limit the length to 1.0f
        float length = Mathf.Sqrt(Model.inputX * Model.inputX + Model.inputY * Model.inputY);
        if (length > 1.0f)
        {
            Model.inputX /= length;
            Model.inputY /= length;
        }

        if (Model.playerState == PLAYER_STATE.Grounded)
        {
            if (Model.falling)
            { // fell on the ground
                Model.falling = false;
                //this.DoFallDamage(Mathf.Abs(Model.fallSpeed));
            }

            // align our Model.movement vectors with the ground normal (ground normal = up)
            Vector3 newForward = Model.cameraPivot.transform.forward;
            Vector3.OrthoNormalize(ref Model.groundNormal, ref newForward);

            Vector3 targetSpeed = Vector3.Cross(Model.groundNormal, newForward) * Model.inputX * PlayerStats.RunSpeed + newForward * Model.inputY * PlayerStats.RunSpeed;

            length = targetSpeed.magnitude;
            float difference = length - Model.rb.velocity.magnitude;

            // avoid divide by zero
            if (Mathf.Approximately(difference, 0.0f))
                Model.movement = Vector3.zero;

            else
            {
                // determine if we should accelerate or decelerate
                if (difference > 0.0f)
                    Model.acceleration = Mathf.Min(PlayerStats.AccelRate * Time.deltaTime, difference);

                else
                    Model.acceleration = Mathf.Max(-PlayerStats.DecelRate * Time.deltaTime, difference);

                // normalize the difference vector and store it in Model.movement
                difference = 1.0f / difference;
                Model.movement = new Vector3((targetSpeed.x - Model.rb.velocity.x) * difference * Model.acceleration, (targetSpeed.y - Model.rb.velocity.y) * difference * Model.acceleration, (targetSpeed.z - Model.rb.velocity.z) * difference * Model.acceleration);
            }

            
            //if (!Model.touchingDynamic && Mathf.Approximately(Model.inputX + Model.inputY, 0.0f))
            //    // prevent sliding by countering gravity... this may be dangerous
            //    Model.movement.y -= Physics.gravity.y * Time.deltaTime;

            Model.rb.AddForce(new Vector3(Model.movement.x, Model.movement.y, Model.movement.z), ForceMode.VelocityChange);
            Model.groundedLastFrame = true;
        }
        else
        {
            if (Model.groundedLastFrame && !Model.falling)
            {
                // see if there's a surface we can stand on beneath us within Model.fudgeCheck range
                if (Physics.Raycast(View.transform.position, Vector3.down, out hit,(Model.rb.velocity.magnitude * Time.deltaTime), ~0) && Vector3.Angle(hit.normal, Vector3.up) <= PlayerStats.MaximumSlope)
                {
                    Model.groundedLastFrame = true;

                    // we can't go straight down, so do another raycast for the exact distance towards the surface
                    // i tried doing exsec and excsc to avoid doing another raycast, but my math sucks and it failed horribly
                    // if anyone else knows a reasonable way to implement a simple trig function to bypass this raycast, please contribute to the thead!
                    if (Physics.Raycast(new Vector3(View.transform.position.x, View.transform.position.y - Model.bottomCapsuleSphereOrigin, View.transform.position.z), -hit.normal, out hit, hit.distance, ~0))
                    {
                        Model.rb.AddForce(hit.normal * -hit.distance, ForceMode.VelocityChange);
                        return; // skip air accel because we should be Model.state == PLAYER_STATES.Grounded
                    }
                }
            }

            // if we're here, we're not fudging so we're defintiely airborne
            // thus, if Model.falling isn't set, set it
            if (!Model.falling)
                Model.falling = true;

            Model.fallSpeed = Model.rb.velocity.y;

            // air accel
            if (!Mathf.Approximately(Model.inputX + Model.inputY, 0.0f))
            {
                // note, this will probably malfunction if you set the air accel too high... this code should be rewritten if you intend to do so

                // get direction vector
                Model.movement = View.transform.TransformDirection(new Vector3(Model.inputX * PlayerStats.AirborneAccel * Time.deltaTime, 0.0f, Model.inputY * PlayerStats.AirborneAccel * Time.deltaTime));

                // add up our accel to the current velocity to check if it's too fast
                float a = Model.movement.x + Model.rb.velocity.x;
                float b = Model.movement.z + Model.rb.velocity.z;

                // check if our new velocity will be too fast
                length = Mathf.Sqrt(a * a + b * b);
                if (length > 0.0f)
                {
                    if (length > PlayerStats.RunSpeed)
                    {
                        // normalize the new Model.movement vector
                        length = 1.0f / Mathf.Sqrt(Model.movement.x * Model.movement.x + Model.movement.z * Model.movement.z);
                        Model.movement.x *= length;
                        Model.movement.z *= length;

                        // normalize our current velocity (before accel)
                        length = 1.0f / Mathf.Sqrt(Model.rb.velocity.x * Model.rb.velocity.x + Model.rb.velocity.z * Model.rb.velocity.z);
                        Vector3 rigidbodyDirection = new Vector3(Model.rb.velocity.x * length, 0.0f, Model.rb.velocity.z * length);

                        // dot product of accel unit vector and velocity unit vector, clamped above 0 and inverted (1-x)
                        length = (1.0f - Mathf.Max(Model.movement.x * rigidbodyDirection.x + Model.movement.z * rigidbodyDirection.z, 0.0f)) * PlayerStats.AirborneAccel * Time.deltaTime;
                        Model.movement.x *= length;
                        Model.movement.z *= length;
                    }

                    // and finally, add our force
                    Model.rb.AddForce(new Vector3(Model.movement.x, 0.0f, Model.movement.z), ForceMode.VelocityChange);
                }
            }

            Model.groundedLastFrame = false;
        }

        // Execute locomotion in physics update.
        //LocomotionV3();
        if(LCManager.LeftModel.isJump && LCManager.RightModel.isJump)
        {
            PlayerJump();
            LCManager.LeftModel.isJump = false;
            LCManager.RightModel.isJump = false;
        }

        // test if in the air and play sound accordingly.
        onAir?.Invoke(Model.playerState);
    }

    public void Update()
    {
        UpdateLookRotation();
    }

    private void UpdateLookRotation()
    {
        var x = Input.GetAxis("Mouse Y");
        var y = Input.GetAxis("Mouse X");

        x *= Model.invertY ? -1 : 1;

        Model.targetPivotRotation = Model.cameraPivot.transform.rotation * Quaternion.AngleAxis(x * Model.lookSpeed * Time.deltaTime, Vector3.right) * 
            Quaternion.AngleAxis(y * Model.lookSpeed * Time.deltaTime, Vector3.up);
        Model.cameraPivot.transform.rotation = Quaternion.Slerp(Model.cameraPivot.transform.rotation, Model.targetPivotRotation, Time.deltaTime * 500);
        float z = Model.cameraPivot.transform.eulerAngles.z;
        Model.cameraPivot.transform.Rotate(0, 0, -z);
    }

    private void UpdateVelocity()
    {
        if (!Mathf.Approximately(Model.rb.velocity.x, 0.0f) && !Model.isPushed)
        {
            Vector3 targetVelocity = Vector3.Lerp(Model.rb.velocity, Model.cameraPivot.forward * Model.rb.velocity.magnitude, Time.deltaTime);
            Model.rb.velocity = new Vector3(targetVelocity.x, Model.rb.velocity.y, targetVelocity.z);
        }
    }

    public void SetState(PLAYER_STATE state)
    {
        Model.playerState = state;
    }

    public void PlayerJump()
    {
        PlayerView.Instance.Model.inAir = true;
        Model.rb.AddForce(Model.cameraPivot.forward.x * PlayerStats.JumpFactor, PlayerStats.JumpSpeed, Model.cameraPivot.forward.z * PlayerStats.JumpFactor, ForceMode.Impulse);
        Model.playerState = PLAYER_STATE.Air;
        onJump?.Invoke();
    }

    public void PlayerStep()
    {
        AudioManager.Instance.SFXWalk();
        if (Model.rb.velocity.magnitude <= 22.0f && !GameplayUIManager.Instance.isCountdown)
        {
            Model.rb.AddForce(Model.cameraPivot.forward.normalized.x * PlayerStats.RunSpeed, 0.0f, Model.cameraPivot.forward.normalized.z * PlayerStats.RunSpeed, ForceMode.VelocityChange);
        }
    }

    public void LocomotionV3()
    {
        if (Model.StepQueue.Count > 0)
        {
            if (Model.locomotionState == LOCOMOTION_STATE.Left)
            {
                Model.StepQueue.RemoveAll(x => x == LOCOMOTION_STATE.Left.ToString());
            }
            else
            {
                Model.StepQueue.RemoveAll(x => x == LOCOMOTION_STATE.Right.ToString());
            }
            Model.StepQueue.RemoveRange(1, Model.StepQueue.Count - 1);
            switch (Model.locomotionState)
            {
                case LOCOMOTION_STATE.None:
                    Model.locomotionState = Model.StepQueue[0] == StepType.Left ? LOCOMOTION_STATE.Left : LOCOMOTION_STATE.Right;
                    PlayerStep();
                    Model.StepQueue.Clear();
                    return;
                case LOCOMOTION_STATE.Left:
                    Model.locomotionState = Model.StepQueue[0] == StepType.Left ? LOCOMOTION_STATE.Left : LOCOMOTION_STATE.Right;
                    if (Model.StepQueue[0] == StepType.Right)
                        PlayerStep();
                    Model.StepQueue.Clear();
                    return;
                case LOCOMOTION_STATE.Right:
                    Model.locomotionState = Model.StepQueue[0] == StepType.Left ? LOCOMOTION_STATE.Left : LOCOMOTION_STATE.Right;
                    if (Model.StepQueue[0] == StepType.Left)
                        PlayerStep();
                    Model.StepQueue.Clear();
                    return;
                default:
                    break;
            }
            LC_CONST.lastStepTime += Time.fixedDeltaTime;
            if(LC_CONST.lastStepTime >= LC_CONST.RESET_TIME)
            {
                LC_CONST.lastStepTime = 0.0f;
                Model.locomotionState = LOCOMOTION_STATE.None;
            }

        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        PlayerStats.DistanceToBelow = 0;
        // keep track of collision objects and contact points
        ContactPoint contact = collision.contacts[0];
        if(Vector3.Dot(contact.normal, Vector3.up) > 0.5)
        {
            if ((collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Box" || collision.gameObject.tag == "Building"))
            {
                Model.collisions.Add(collision.gameObject);
                Model.contactPoints.Add(collision.gameObject.GetInstanceID(), collision.contacts);
                SetState(PLAYER_STATE.Grounded);
                Model.isPushed = false;
                Model.inAir = false;
                AudioManager.Instance.SFXLanding();
                LCManager.RightModel.isJump = false;
                LCManager.LeftModel.isJump = false;
            }
        }
        // check if this object is dynamic
        if (!collision.gameObject.isStatic)
            Model.touchingDynamic = true;

        if(collision.gameObject.tag == "Vehicle")
            AudioManager.Instance.SFXHit();

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Mail" || other.gameObject.tag == "Coin")
        {
            Collectible c = other.gameObject.GetComponent<Collectible>();
            ScoreManager.Instance.Score(c);
            other.gameObject.SetActive(false);
            return;
        }

        if (other.gameObject.tag == "RespawnArea")
        {
            RespawnManager.Instance.Respawn();
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        // update contact points
        ContactPoint contact = collision.contacts[0];
        if ((collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Box" || collision.gameObject.tag == "Building")/* && Model.currentTriggerCollider == default*/
             && contact.normal == Vector3.up)
        {
            SetState(PLAYER_STATE.Grounded);
        }
        if (Model.playerState == PLAYER_STATE.Grounded)
            Model.contactPoints[collision.gameObject.GetInstanceID()] = collision.contacts;
        if (collision.gameObject.tag == "Box")
        {
            PlayerStats.LastContactNormal = collision.contacts[0].normal;
            PlayerStats.LastContactTag = collision.gameObject.tag;
        }
            
        if (collision.gameObject.tag == "Building" && collision.contacts[0].normal != Vector3.up)
        {
            PlayerStats.LastContactNormal = collision.contacts[0].normal;
            PlayerStats.LastContactTag = collision.gameObject.tag;
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Box" || (collision.gameObject.tag == "Building" && PlayerStats.LastContactNormal != Vector3.up))
            Model.rb.AddForce(-PlayerStats.LastContactNormal * 5.5f, ForceMode.Impulse);
        // remove this collision and its associated contact points from the list
        // don't break from the list once we find it because we might somehow have duplicate entries, and we need to recheck groundedOnDynamic anyways
        for (int i = 0; i < Model.collisions.Count; i++)
        {
            if (Model.collisions[i] == collision.gameObject)
                Model.collisions.RemoveAt(i--);

            else if (!Model.collisions[i].isStatic)
                Model.touchingDynamic = true;
        }

        Model.contactPoints.Remove(collision.gameObject.GetInstanceID());
    }

    public bool falling
    {
        get
        {
            return Model.falling;
        }
    }

    public float fallSpeed
    {
        get
        {
            return Model.fallSpeed;
        }
    }

    public Vector3 groundNormal
    {
        get
        {
            return Model.groundNormal;
        }
    }

    //public void DoFallDamage(float fallSpeed) // fallSpeed will be positive
    //{
    //    // do your fall logic here using Model.fallSpeed to determine how hard we hit the ground
    //    //Debug.Log("Hit the ground at " + fallSpeed.ToString() + " units per second");
    //}

    public void Reset(VirtualScene scene)
    {
        Model.rb.velocity = Vector3.zero;
        Model.rb.position = scene.initialSpawn;
        Model.rb.transform.rotation = Quaternion.identity;
        Model.contactPoints.Clear();
        Model.cameraPivot.rotation = Quaternion.identity;
    }
}