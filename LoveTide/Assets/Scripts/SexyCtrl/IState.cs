public interface IState
{ 
     void OnEnterState(object action);
     void OnStayState(object action);
     void OnExitState(object action);
}