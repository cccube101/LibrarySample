using Cysharp.Threading.Tasks;
using DG.Tweening;
using Helper;
using R3;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UIHelper;

public class TitleManager : MonoBehaviour
{
    // ---------------------------- SerializeField
    [SerializeField] private GameObject _fadeObj;
    [SerializeField] private GameObject _titleObj;
    [SerializeField] private GameObject _stageSelectObj;

    [SerializeField] private Button _startBtn;
    [SerializeField] private Button _exitBtn;

    [SerializeField] private GameObject[] _stageButtons;
    [SerializeField] private Button _backBtn;

    // ---------------------------- Field
    private GroupParam _fadeParam;
    private RectParam _titleParam;
    private RectParam _stageSelectParam;

    private bool _isStart = false;

    private SceneName _loadScene = SceneName.Stage01;

    // ---------------------------- UnityMessage
    private void Awake()
    {
        _fadeParam = new GroupParam(_fadeObj);
        _titleParam = new RectParam(_titleObj);
        _stageSelectParam = new RectParam(_stageSelectObj);
    }

    private async void Start()
    {
        StateObserve();
        BtnObserve();

        await Tasks.Canceled(StartEvent(destroyCancellationToken));
    }

    // ---------------------------- PrivateMethod
    #region StateObserve
    private void StateObserve()
    {
        Data.GameState.SubscribeAwait(async (state, ct) =>
        {
            switch (state)
            {
                case GameState.Title:
                    await TitleEvent(ct);
                    break;

                case GameState.StageSelect:
                    await StageSelectEvent(ct);
                    break;

                case GameState.GamePlay:
                    await GamePlayEvent(ct);
                    break;

                case GameState.End:
                    await ExitEvent(ct);
                    break;

            }
        }, AwaitOperation.Switch)
        .RegisterTo(destroyCancellationToken);
    }

    private async UniTask TitleEvent(CancellationToken ct)
    {
        if (!_isStart) return;

        await new List<UniTask>()
         {
             _titleParam.rect.DOAnchorPosY(0, 0.5f)
                 .SetEase(Ease.OutBack)
                 .SetLink(_titleParam.obj)
                 .ToUniTask(Tasks.TCB, ct),

             _stageSelectParam.rect.DOAnchorPosY(-1200f, 0.5f)
                 .SetEase(Ease.OutBack)
                 .SetLink(_stageSelectParam.obj)
                 .ToUniTask(Tasks.TCB, ct),
         };
        _startBtn.interactable = true;
    }

    private async UniTask StageSelectEvent(CancellationToken ct)
    {
        _startBtn.interactable = false;
        await new List<UniTask>()
         {
             _titleParam.rect.DOAnchorPosY(1200, 0.5f)
                 .SetEase(Ease.OutBack)
                 .SetLink(_titleParam.obj)
                 .ToUniTask(Tasks.TCB, ct),

             _stageSelectParam.rect.DOAnchorPosY(0f, 0.5f)
                 .SetEase(Ease.OutBack)
                 .SetLink(_stageSelectParam.obj)
                 .ToUniTask(Tasks.TCB, ct),
         };
    }

    private async UniTask GamePlayEvent(CancellationToken ct)
    {
        await Fade(1f, ct);
        SceneManager.LoadScene((int)_loadScene);
    }

    private async UniTask ExitEvent(CancellationToken ct)
    {
        await Fade(1f, ct);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //ゲームシーン終了
#else
        Application.Quit(); //build後にゲームプレイ終了が適用
#endif
    }

    #endregion

    private void BtnObserve()
    {
        _startBtn.onClick.AddListener(() => Data.SetGameState(GameState.StageSelect));
        _backBtn.onClick.AddListener(() => Data.SetGameState(GameState.Title));
        _exitBtn.onClick.AddListener(() => Data.SetGameState(GameState.End));

        foreach (var item in Data.SceneNames)
        {
            if (item == SceneName.Title) continue;
            if (item == SceneName.BaseScene) break;

            var obj = _stageButtons[(int)item - 1];
            obj.GetComponentInChildren<TMP_Text>().SetText(item.ToString());
            obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                _loadScene = item;
                Data.SetGameState(GameState.GamePlay);
            });
        }
    }

    private async UniTask StartEvent(CancellationToken ct)
    {
        await Fade(0f, ct);
        _isStart = true;
    }

    private async UniTask Fade(float to, CancellationToken ct)
    {
        await _fadeParam.group.DOFade(to, 1f)
              .SetEase(Ease.Linear)
              .SetLink(_fadeParam.obj)
              .ToUniTask(Tasks.TCB, ct);
    }
}