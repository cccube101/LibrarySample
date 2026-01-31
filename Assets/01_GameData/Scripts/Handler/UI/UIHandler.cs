using Cysharp.Threading.Tasks;
using DG.Tweening;
using Helper;
using R3;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UIHelper;

public class UIHandler : MonoBehaviour
{
    // ---------------------------- SerializeField
    [HorizontalLine("Base")]
    [SerializeField] private GameObject _fadeObj;
    [SerializeField] private GameObject _menuObj;

    [SerializeField] private TMP_Text _stateText;
    [SerializeField] private TMP_Text _scoreText;

    [HorizontalLine("Menu")]
    [SerializeField] private Button _menuBtn;
    [SerializeField] private Button _backBtn;
    [SerializeField] private Button _titleBtn;
    [SerializeField] private Button _stageSelectBtn;

    [HorizontalLine("Player")]
    [SerializeField] private TMP_Text _hpText;

    // ---------------------------- Field
    private GameObject _obj;
    private float _currentScore = 0f;
    private float _currentHp = 0f;

    private GroupParam _fadeParam;
    private RectParam _menuParam;
    private float _menuOriginY;

    // ---------------------------- UnityMessage
    private void Awake()
    {
        _obj = gameObject;
        _fadeParam = new GroupParam(_fadeObj);
        _menuParam = new RectParam(_menuObj);
        _menuOriginY = _menuParam.rect.anchoredPosition.y;
    }
    private async void Start()
    {
        StateObserve();
        BtnObserve();
        TextUpdate();

        await Tasks.Canceled(StartEvent(destroyCancellationToken));
    }



    // ---------------------------- PublicMethod



    // ---------------------------- PrivateMethod
    private void StateObserve()
    {
        Data.GameState.SubscribeAwait(async (state, ct) =>
        {
            switch (state)
            {
                case GameState.Title:
                case GameState.StageSelect:
                    await Fade(1f, ct);
                    Data.SetPlayState(PlayState.Play);
                    Time.timeScale = 1f;
                    SceneManager.LoadScene((int)SceneName.Title);
                    break;
            }

        }, AwaitOperation.Drop)
        .RegisterTo(destroyCancellationToken);

        Data.PlayState.SubscribeAwait(async (state, ct) =>
        {
            switch (state)
            {
                case PlayState.Play:
                    await MovePos(_menuOriginY, ct);
                    Time.timeScale = 1f;
                    break;

                case PlayState.Pause:
                    Time.timeScale = 0f;
                    await MovePos(0, ct);
                    break;
            }

        }, AwaitOperation.Drop)
        .RegisterTo(destroyCancellationToken);

        async UniTask MovePos(float pos, CancellationToken ct)
        {
            await _menuParam.rect.DOAnchorPosY(pos, 0.5f)
                .SetEase(Ease.OutBack)
                .SetLink(_menuParam.obj)
                .SetUpdate(true)
                .ToUniTask(Tasks.TCB, ct);
        }
    }

    private void BtnObserve()
    {
        _menuBtn.onClick.AddListener(() => Data.SetPlayState(PlayState.Pause));
        _backBtn.onClick.AddListener(() => Data.SetPlayState(PlayState.Play));

        _titleBtn.onClick.AddListener(() => Data.SetGameState(GameState.Title));
        _stageSelectBtn.onClick.AddListener(() => Data.SetGameState(GameState.StageSelect));
    }

    private void TextUpdate()
    {
        Data.GameState.Subscribe(state =>
        {
            _stateText.text = $"State {state}";
        })
        .AddTo(this);

        GameManager.Inst.GameData.Score.SubscribeAwait(async (score, ct) =>
        {
            await DOVirtual.Float(_currentScore, score, 0.1f
            , value =>
            {
                _scoreText.text = $"Score {GetSuffix(value)}";
            })
            .SetEase(Ease.Linear)
            .SetLink(_obj)
            .ToUniTask(Tasks.TCB, ct);

            _currentScore = score;

        }, AwaitOperation.Drop)
        .RegisterTo(destroyCancellationToken);

        GameManager.Inst.Player.Hp.SubscribeAwait(async (hp, ct) =>
        {
            await DOVirtual.Float(_currentHp, hp, 0.1f
            , value =>
            {
                _hpText.text = $"HP {GetSuffix(value)}";
            })
            .SetEase(Ease.Linear)
            .SetLink(_obj)
            .ToUniTask(Tasks.TCB, ct);

            _currentHp = hp;

        }, AwaitOperation.Drop)
        .RegisterTo(destroyCancellationToken);
    }

    private async UniTask StartEvent(CancellationToken ct)
    {
        Time.timeScale = 0f;
        await Fade(0f, ct);
        Time.timeScale = 1f;
    }

    private async UniTask Fade(float to, CancellationToken ct)
    {
        await _fadeParam.group.DOFade(to, 1f)
              .SetEase(Ease.Linear)
              .SetLink(_fadeParam.obj)
              .SetUpdate(true)
              .ToUniTask(Tasks.TCB, ct);
    }
}