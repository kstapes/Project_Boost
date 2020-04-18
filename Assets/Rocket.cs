using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] private float rcsThrust = 100f;
    [SerializeField] private float mainThrust = 10f;
    [SerializeField] private AudioClip mainEngine;
    [SerializeField] private AudioClip success;
    [SerializeField] private AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    private Rigidbody rigidBody;
    private AudioSource audioSource;

    private enum State
    { Alive, Dying, Transcending }

    private State state = State.Alive;

    //bool ButtonsEnabled = true; first pass at disabling buttons when player dies

    // Start is called before the first frame update
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing if friendly
                break;

            case "Finish":
                StartSuccess();
                break;

            default:
                StartDeath();
                break;
        }
    }

    private void StartDeath()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        //ButtonsEnabled = false;
        Invoke("LoadFirstScene", 1f);
    }

    private void StartSuccess()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        Invoke("LoadNextScene", 1f);
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