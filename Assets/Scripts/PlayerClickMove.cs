using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClickMove : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;

    [Header("Пауза")]
    [SerializeField] private GameObject pauseMenuUI; // Ссылка на UI меню паузы
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    private bool isPaused = false;
    [SerializeField] private bool _canMoveDuringPause = false;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        // Скрываем меню паузы при старте
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

    void Update()
    {

        // Проверка нажатия клавиши паузы
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }

        // Управление движением (только если не на паузе или разрешено)
        if (!isPaused || _canMoveDuringPause)
        {
            HandleMovement();
        }
    }

    void HandleMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Останавливаем время

        // Показываем меню паузы
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        // Отключаем агента на паузе (если не разрешено движение)
        if (!_canMoveDuringPause && agent != null)
            agent.isStopped = true;

        // Разблокируем курсор для меню
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Игра на паузе");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Восстанавливаем время

        // Скрываем меню паузы
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // Включаем агента
        if (agent != null)
            agent.isStopped = false;



        Debug.Log("Игра продолжена");
    }


}
