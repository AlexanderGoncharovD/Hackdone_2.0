using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button : MonoBehaviour {

	public bool isBusy;
	public int id;
	public controller control;

    // выполняется один раз, при добавлении скрипта на объект
	void Reset ( )
	{
		control = GameObject.FindWithTag("MainCamera").GetComponent<controller>( );
        // Присвоение ID точке
		for (int i = 0; i < 9; i++)
			if (control.buttons[i] == gameObject)
				id = i;
	}


	void OnMouseOver ( )
	{
        // Если курсор/палец удерживается на точке пароля И игра уже началась
		if (Input.GetMouseButton(0) && control.isStartedGame)
		{
            // Если точка ещё не занята И пароль не верный И пароль ещё не введен полностью
			if (! isBusy && !control.passwordIsCorrect && control.numInputSymbol < control.difficult)
			{
				control.CreateLine (transform, id); // Запускаем функцию создания линии из этой точки
                // отключаем коллайдер, чтобы это точка больше не реагировала на нажатия
                GetComponent<CircleCollider2D>( ).enabled = false;
			}
		}
	}
}
