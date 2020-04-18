using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 10f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip levelLoad;
    [SerializeField] AudioClip deathSound;

    Rigidbody rigidBody;
    AudioSource audioSource;
    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    //bool ButtonsEnabled = true; first pass at disabling buttons when player dies

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
    }
        
    // Update is called once per frame
    void Update()
    {
        //todo stop sound on death
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive){return;}

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing if friendly
                break;
            case "Finish":
                state = State.Transcending;
                Invoke("LoadNextScene", 1f);
                break;
            default:
                print("Hit something deadly");
                audioSource.Stop();
                audioSource.PlayOneShot(deathSound);
                state = State.Dying;
                //ButtonsEnabled = false;
                Invoke("LoadFirstScene", 1f);
                break;
        }
    }
    private void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
        //ButtonsEnabled = true;
    }
    private void LoadNextScene()
    {

        SceneManager.LoadScene(1);
       
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space)) //&& ButtonsEnabled == true
        {
            ApplyThrust();

        }
        else
        {
            audioSource.Stop();
        }
    }

    private void ApplyThrust()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; //take manual control of rotation 
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) //&& ButtonsEnabled == true
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))//&& ButtonsEnabled == true
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; //resume physics control of rotation
    }

   
}
