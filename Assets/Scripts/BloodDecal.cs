using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(DecalProjector))]
public class BloodDecal : MonoBehaviour, IPoolable
{
    private static readonly LinkedList<BloodDecal> ActiveDecals = new LinkedList<BloodDecal>();

    [SerializeField] private int _maxActive = 80;
    [SerializeField] private float _lifetime = 45f;
    [SerializeField] private float _fadeDuration = 4f;

    private DecalProjector _projector;
    private LinkedListNode<BloodDecal> _node;
    private float _age;

    private void Awake()
    {
        _projector = GetComponent<DecalProjector>();
        _projector.pivot = Vector3.zero;
    }

    public void Place(Vector3 point, Vector3 normal, float size, float depth, int variant, int columns, int rows)
    {
        Quaternion rotation = Quaternion.LookRotation(-normal) * Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        transform.SetPositionAndRotation(point, rotation);

        _projector.size = new Vector3(size, size, depth);

        float tileWidth = 1f / columns;
        float tileHeight = 1f / rows;
        int column = variant % columns;
        int row = (variant / columns) % rows;

        _projector.uvScale = new Vector2(tileWidth, tileHeight);
        _projector.uvBias = new Vector2(column * tileWidth, row * tileHeight);
        _projector.fadeFactor = 1f;

        _age = 0f;
    }

    public void OnSpawnedFromPool()
    {
        _node = ActiveDecals.AddLast(this);

        while (ActiveDecals.Count > _maxActive)
        {
            BloodDecal oldest = ActiveDecals.First.Value;

            if (oldest == this)
                break;

            PooledObject.Despawn(oldest.gameObject);
        }
    }

    public void OnReturnedToPool()
    {
        if (_node != null && _node.List != null)
            ActiveDecals.Remove(_node);

        _node = null;
    }

    private void Update()
    {
        _age += Time.deltaTime;

        float fadeStart = _lifetime - _fadeDuration;

        if (_age >= fadeStart)
            _projector.fadeFactor = Mathf.Clamp01((_lifetime - _age) / Mathf.Max(_fadeDuration, 0.01f));

        if (_age >= _lifetime)
            PooledObject.Despawn(gameObject);
    }
}