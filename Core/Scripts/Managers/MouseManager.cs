using Cards;
using Godot;
using System;
using DI;
using System.Reflection;

public interface IPointerManager
{
    event Action onMove;
    event Action onPointerDown;
    event Action onPointerUp;
    event Action onDoubleTap;

    Vector2 NowScreenPosition { get; }
    Vector2 NowWorldPosition { get; }
    T Raycast<T>() where T : class;
    object RaycastByObjType(object obj);
    object RaycastByType(Type type);

}

public partial class MouseManager : Node2D, IPointerManager
{
    [Inject] private IObjectResolver objectResolver;

    private const float tapThreshold = 0.25f;
    private double tapTimer = 0.0;
    private bool tap = false;

    private Vector2 mousePosition;
    private Vector2 mouseDelta;
    private Vector2 lastClickPos;
    private bool isMouseDown;
    
    private bool gotMouseDown = false;
    private bool gotMouseUp = false;

    public Vector2 NowScreenPosition => GetViewport().GetMousePosition();

    public Vector2 NowWorldPosition => GetGlobalMousePosition();

    public event Action onMove;
    public event Action onPointerDown;
    public event Action onPointerUp;
    public event Action onDoubleTap;

    public override void _Ready()
    {
        gotMouseDown = false;
        gotMouseUp = false;
        isMouseDown = false;
        tap = false;

        mousePosition = GetGlobalMousePosition();
        mouseDelta = Vector2.Zero;
    }

    public override void _Process(double delta)
    {
        GetMouseValues();
        CheckEvents();
    }

    public override void _Input(InputEvent @event)
    {
        var mouseEvent = @event as InputEventMouseButton;
        if (mouseEvent == null || mouseEvent.ButtonIndex != MouseButton.Left)
            return;
        InputCheckMouseEvents(mouseEvent);
    }

    private void GetMouseValues()
    {
        mousePosition = NowWorldPosition;
        mouseDelta = NowWorldPosition - mousePosition;
    }

    private void CheckEvents()
    {
        if (mouseDelta.Length() >= 0.0f)
        {
            onMove?.Invoke();
        }

        if (!isMouseDown && tap == true && Time.Singleton.GetUnixTimeFromSystem() > tapTimer + tapThreshold)
        {
            tap = false;
        }
    }

    private void InputCheckMouseEvents(InputEventMouseButton mouseEvent)
    {
        if (isMouseDown && mouseEvent.IsReleased())
        {
            isMouseDown = false;
            onPointerUp?.Invoke();
        }


        if (!isMouseDown && mouseEvent.IsPressed())
        {
            lastClickPos = NowWorldPosition;
            isMouseDown = true;

            gotMouseDown = true;
            onPointerDown?.Invoke();

            if (Time.Singleton.GetUnixTimeFromSystem() < tapTimer + tapThreshold)
            {
                tap = false;

                onDoubleTap?.Invoke();
                return;
            }
            tap = true;
            tapTimer = Time.Singleton.GetUnixTimeFromSystem();
        }
    }
    
    public T Raycast<T>() where T : class //not working with interfaces
    {
        var spaceState = GetWorld2D().DirectSpaceState;
        var parameters = new PhysicsPointQueryParameters2D();
        parameters.Position = NowWorldPosition;
        parameters.CollideWithAreas = true;
        parameters.CollisionMask = 1;

        var results = spaceState.IntersectPoint(parameters);

        if (results.Count > 0)
        {
            foreach (var result in results)
            {
                Area2D collider = result["collider"].As<Area2D>();
                T val = collider.GetParent() as T;

                if (val != null)
                {
                    return val;
                }
            }
        }

        return null;
    }

    public object RaycastByObjType(object obj)
    {
        Type type = obj.GetType();
        MethodInfo method = GetType().GetMethod("Raycast");
        method = method.MakeGenericMethod(type);
        return method.Invoke(this, null);
    }

    public object RaycastByType(Type type)
    {
        MethodInfo method = GetType().GetMethod("Raycast");
        method = method.MakeGenericMethod(type);
        return method.Invoke(this, null);
    }
}
