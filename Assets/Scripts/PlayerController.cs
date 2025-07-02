using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public enum State { Standing, LyingX, LyingZ }
    public State currentState = State.Standing;

    public float rollSpeed = 5f;
    private bool isRolling = false;

    private Vector3 currentPos;

    // Kích thước khối
    const float WIDTH = 2f;
    const float HEIGHT = 4f;
    const float DEPTH = 2f;

    // UI Panels
    public GameObject winPanel;
    public GameObject losePanel;

    // Grid (ẩn khi thắng/thua)
    public GameObject gridRoot;

    // Quản lý level
    public LevelManager levelManager;

    void Start()
    {
        currentPos = transform.position;
        SnapToGrid();
    }

    public void MoveUp() => TryMove(Vector3.forward);
    public void MoveDown() => TryMove(Vector3.back);
    public void MoveLeft() => TryMove(Vector3.left);
    public void MoveRight() => TryMove(Vector3.right);

    void TryMove(Vector3 dir)
    {
        if (!isRolling)
            StartCoroutine(Roll(dir));
    }

    IEnumerator Roll(Vector3 dir)
    {
        isRolling = true;

        Vector3 anchor = Vector3.zero;
        Vector3 axis = Vector3.Cross(Vector3.up, dir);
        Vector3 newPos = currentPos;
        State nextState = currentState;

        if (currentState == State.Standing)
        {
            anchor = currentPos + (Vector3.down * (HEIGHT / 2f)) + (dir * (WIDTH / 2f));
            newPos += dir * HEIGHT;
            nextState = (dir == Vector3.left || dir == Vector3.right) ? State.LyingX : State.LyingZ;
        }
        else if (currentState == State.LyingX)
        {
            if (dir == Vector3.left)
            {
                anchor = currentPos + Vector3.left * (HEIGHT / 2f) + Vector3.down * (WIDTH / 2f);
                newPos += Vector3.left * WIDTH;
                nextState = State.Standing;
            }
            else if (dir == Vector3.right)
            {
                anchor = currentPos + Vector3.right * (HEIGHT / 2f) + Vector3.down * (WIDTH / 2f);
                newPos += Vector3.right * WIDTH;
                nextState = State.Standing;
            }
            else if (dir == Vector3.forward || dir == Vector3.back)
            {
                float dirZ = dir == Vector3.forward ? 1f : -1f;
                anchor = currentPos + Vector3.forward * dirZ * (WIDTH / 2f) + Vector3.down * (WIDTH / 2f);
                newPos += Vector3.forward * dirZ * WIDTH;
                nextState = State.LyingX;
            }
        }
        else if (currentState == State.LyingZ)
        {
            if (dir == Vector3.forward)
            {
                anchor = currentPos + Vector3.forward * (HEIGHT / 2f) + Vector3.down * (DEPTH / 2f);
                newPos += Vector3.forward * DEPTH;
                nextState = State.Standing;
            }
            else if (dir == Vector3.back)
            {
                anchor = currentPos + Vector3.back * (HEIGHT / 2f) + Vector3.down * (DEPTH / 2f);
                newPos += Vector3.back * DEPTH;
                nextState = State.Standing;
            }
            else if (dir == Vector3.left || dir == Vector3.right)
            {
                float dirX = dir == Vector3.left ? -1f : 1f;
                anchor = currentPos + Vector3.right * dirX * (DEPTH / 2f) + Vector3.down * (DEPTH / 2f);
                newPos += Vector3.right * dirX * DEPTH;
                nextState = State.LyingZ;
            }
        }

        float angle = 0f;
        while (angle < 90f)
        {
            float step = rollSpeed * Time.deltaTime * 90f;
            float delta = Mathf.Min(step, 90f - angle);
            transform.RotateAround(anchor, axis, delta);
            angle += delta;
            yield return null;
        }

        currentPos = newPos;
        currentState = nextState;
        SnapToGrid();

        isRolling = false;
    }

    void SnapToGrid()
    {
        currentPos.x = Mathf.Round(currentPos.x * 100f) / 100f;
        currentPos.y = Mathf.Round(currentPos.y * 100f) / 100f;
        currentPos.z = Mathf.Round(currentPos.z * 100f) / 100f;
        transform.position = currentPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Win"))
        {
            if (winPanel != null) winPanel.SetActive(true);
            HidePlayerAndGrid();
            Time.timeScale = 0f;
        }
        else if (other.CompareTag("Lose"))
        {
            if (losePanel != null) losePanel.SetActive(true);
            HidePlayerAndGrid();
            Time.timeScale = 0f;
        }
    }

    void HidePlayerAndGrid()
    {
        gameObject.SetActive(false);
        if (gridRoot != null)
            gridRoot.SetActive(false);
    }

    public void Replay()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(true); // Hiển thị lại nhân vật
        if (gridRoot != null) gridRoot.SetActive(true); // Hiển thị lại lưới
        if (winPanel != null) winPanel.SetActive(false); // Ẩn panel thắng
        if (losePanel != null) losePanel.SetActive(false); // Ẩn panel thua
        currentPos = transform.position; // Reset vị trí
        currentState = State.Standing; // Reset trạng thái
        SnapToGrid(); // Đảm bảo vị trí được căn chỉnh
        levelManager?.LoadLevel(levelManager.GetCurrentIndex()); // Tải lại level hiện tại
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(true); // Hiển thị lại nhân vật
        if (gridRoot != null) gridRoot.SetActive(true); // Hiển thị lại lưới
        if (winPanel != null) winPanel.SetActive(false); // Ẩn panel thắng
        if (losePanel != null) losePanel.SetActive(false); // Ẩn panel thua
        currentPos = transform.position; // Reset vị trí
        currentState = State.Standing; // Reset trạng thái
        SnapToGrid(); // Đảm bảo vị trí được căn chỉnh
        int nextIndex = (levelManager.GetCurrentIndex() + 1) % levelManager.GetLevelsLength(); // Tính chỉ số level tiếp theo
        levelManager?.LoadLevel(nextIndex); // Tải level tiếp theo
    }
}


