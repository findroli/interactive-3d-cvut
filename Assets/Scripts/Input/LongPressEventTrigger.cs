using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LongPressEventTrigger : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
    [ Tooltip( "How long must pointer be down on this object to trigger a long press" ) ]
    public float durationThreshold = 1.6f;
 
    public UnityEvent onLongPressStart = new UnityEvent();
    public UnityEvent onLongPressEnd = new UnityEvent();
 
    private bool isPointerDown = false;
    private bool longPressTriggered = false;
    private float timePressStarted;
 
    private void Update( ) {
        if ( isPointerDown && !longPressTriggered ) {
            if ( Time.time - timePressStarted > durationThreshold ) {
                longPressTriggered = true;
                onLongPressStart.Invoke( );
            }
        }
        else if (!isPointerDown && longPressTriggered) {
            longPressTriggered = false;
            onLongPressEnd.Invoke();
        }
    }
 
    public void OnPointerDown( PointerEventData eventData ) {
        timePressStarted = Time.time;
        isPointerDown = true;
        longPressTriggered = false;
    }
 
    public void OnPointerUp( PointerEventData eventData ) {
        isPointerDown = false;
    }
 
    public void OnPointerExit( PointerEventData eventData ) {
        //isPointerDown = false;
    }
}