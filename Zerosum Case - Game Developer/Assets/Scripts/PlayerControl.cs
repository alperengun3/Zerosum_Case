using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerControl : MonoBehaviour
{
    Rigidbody rb;
    Animator anim;
    Vector3 mousePos;
    Vector3 firstPos;
    Vector3 mouseDif;
    float currentDist;
    float startDist;
    [SerializeField] float currentDiamond;
    [SerializeField] GameObject finishPos;
    [SerializeField] Image imageBar;
    [SerializeField] Image imageBarDiamond;
    [SerializeField] Camera ortho;
    [SerializeField] PlayerSettings settings;
    [SerializeField] GameObject boostParticle;
    [SerializeField] GameObject stackBarFill;
    [SerializeField] GameObject stackBar;
    Canvas canvas;

    void Start()
    {
        GameObject finishTransform = GameObject.FindGameObjectWithTag("Finish");
        finishPos = finishTransform;
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        anim.SetBool(StringClass.TAG_ISIDLE, true);
        anim.SetBool(StringClass.TAG_ISRUNNING, false);
        anim.SetBool(StringClass.TAG_ISBOOSTRUN, false);
        anim.SetBool(StringClass.TAG_ISSTUN, false);
        settings.isPlaying = false;
        startDist = Vector3.Distance(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(0, 0, finishPos.transform.position.z));
        currentDiamond = 0;
        settings.isBoost = false;
        settings.isFinish = false;
        settings.isStun = false;
        settings.diamondBoostCount = 0;
        settings.diamondCount = 0;
        boostParticle.SetActive(false);
        stackBar.SetActive(true);
        canvas = FindObjectOfType<Canvas>();
    }

    void Update()
    {
        currentDist = (Vector3.Distance(new Vector3(0, 0, transform.position.z), new Vector3(0, 0, finishPos.transform.position.z))) / startDist;
        imageBar.fillAmount = 1 - currentDist;

        if (settings.diamondCount == 8)
        {
            currentDiamond -= Time.deltaTime * 2.8f;
            Invoke("DiamondBoost", 3);
        }

        imageBarDiamond.fillAmount =  (currentDiamond / 8);

        if (settings.diamondBoostCount <= -0.2f)
        {
            settings.isBoost = true;

            if (!settings.isFinish && !settings.isStun && settings.isBoost)
            {
                anim.SetBool(StringClass.TAG_ISRUNNING, false);
                anim.SetBool(StringClass.TAG_ISBOOSTRUN, true);
                settings.diamondBoostCount = -0.2f;
                settings.ForwardSpeed = 15;
                boostParticle.SetActive(true);
                StartCoroutine(BoostEnd());
            }
        }
        else
        {
            if (!settings.isFinish)
            {
                settings.ForwardSpeed = 7.5f;
            }
        }

        if (!settings.isPlaying)
        {
            anim.SetBool(StringClass.TAG_ISRUNNING, false);
            anim.SetBool(StringClass.TAG_ISBOOSTRUN, false);
        }

        if (Input.GetMouseButtonDown(0) && !settings.isFinish && !settings.isStun && !settings.isBoost)
        {
            anim.SetBool(StringClass.TAG_ISIDLE, false);
            anim.SetBool(StringClass.TAG_ISRUNNING, true);
            settings.isPlaying = true;
        }

        if (settings.isPlaying)
        {
            Move();
        }

        if (settings.isPlaying)
        {
            firstPos = Vector3.Lerp(firstPos, mousePos, 0.1f);

            if (Input.GetMouseButtonDown(0))
                MouseDown(Input.mousePosition);

            else if (Input.GetMouseButtonUp(0))
                MouseUp();

            else if (Input.GetMouseButton(0))
                MouseHold(Input.mousePosition);
        }
    }

    void MouseDown(Vector3 inputPos)
    {
        mousePos = ortho.ScreenToWorldPoint(inputPos);
        firstPos = mousePos;
    }

    void MouseHold(Vector3 inputPos)
    {
        mousePos = ortho.ScreenToWorldPoint(inputPos);
        mouseDif = mousePos - firstPos;
        mouseDif *= settings.sensitivity;
    }

    void MouseUp()
    {
        mouseDif = Vector3.zero;
    }

    void Move()
    {
        if (settings.isPlaying)
        {
            float xPos = Mathf.Clamp(transform.position.x, -2f, 2f);
            transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
            rb.velocity = new Vector3(mouseDif.x, rb.velocity.y, 1 * settings.ForwardSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(StringClass.TAG_FINISH))
        {
            canvas.coin.SetActive(false);
            transform.DORotate(new Vector3(0, 180, 0), 0.4f);
            transform.DOMoveX(0, 0.75f);
            settings.isFinish = true;
            anim.SetBool(StringClass.TAG_ISBOOSTRUN, false);
            anim.SetBool(StringClass.TAG_ISRUNNING, false);
            anim.SetBool(StringClass.TAG_ISSTUN, false);
            anim.SetBool(StringClass.TAG_ISDANCE, true);
            boostParticle.SetActive(false);
            settings.isPlaying = false;
            rb.isKinematic = true;
            stackBar.SetActive(false);
            canvas.levelComplete();
        }

        if (other.gameObject.CompareTag(StringClass.TAG_OBSTACLE))
        {
            anim.SetBool(StringClass.TAG_ISRUNNING, false);
            anim.SetBool(StringClass.TAG_ISBOOSTRUN, false);
            anim.SetBool(StringClass.TAG_ISSTUN, true);
            settings.ForwardSpeed = 7.5f;
            boostParticle.SetActive(false);
            settings.isStun = true;
            settings.diamondBoostCount = 0;
            settings.diamondCount = 0;
            currentDiamond = 0;
            Invoke("StunEnd", 0.5f);
        }

        if (other.gameObject.CompareTag(StringClass.TAG_DIAMOND))
        {
            if (settings.isBoost == false)
            {
                settings.diamondBoostCount -= 0.025f;
                settings.diamondCount++;
                currentDiamond++;
            }
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag(StringClass.TAG_GOLD))
        {
            other.gameObject.SetActive(false);
            canvas.CoinCollect();
            settings.coin += 5;
        }

        if (other.gameObject.CompareTag(StringClass.TAG_PREFSRESET))
        {
            settings.coin = 0;
            settings.newCoin = 0;
            settings.sensitivity = 7.5f;
        }
    }

    IEnumerator BoostEnd()
    {
        yield return new WaitForSeconds(3);
        settings.diamondBoostCount = 0;
        settings.ForwardSpeed = 7.5f;
        boostParticle.SetActive(false);
        settings.isBoost = false;
        anim.SetBool(StringClass.TAG_ISBOOSTRUN, false);
        if (!settings.isStun)
        {
            anim.SetBool(StringClass.TAG_ISRUNNING, true);
        }
        
    }

    void StunEnd()
    {
        settings.isStun = false;
        anim.SetBool(StringClass.TAG_ISRUNNING, true);
        anim.SetBool(StringClass.TAG_ISSTUN, false);
    }

    void DiamondBoost()
    {
        settings.diamondCount = 0;
        currentDiamond = 0;
    }
}
