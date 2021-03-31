using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.Utility;

namespace SevenSwords.CharacterCore {
	public class StateInputs
	{
		public enum Inputs { vertical, horizontal, attack, attack2, jump }
		private List<(Inputs input, object arg)> inputList = new List<(Inputs, object)>();
		public List<(Inputs input, object arg)> _inputList { get => inputList;}

		public void InputUpdate()
		{

		}

		public void clearList()
		{
			inputList.Clear();
		}

		/// <summary>
		/// Horizontal movement controls
		/// </summary>
		/// <param name="xValue"> position of axis between -1 and 1 </param>
		public void horizontalAxis (float xValue)
		{
			inputList.Add((Inputs.horizontal, xValue));
		}

		public void jump (float value)
		{
			inputList.Add((Inputs.jump, value));
		}

		public void attack(Hitbox hitbox)
		{
			inputList.Add((Inputs.attack, hitbox));
		}
}
}

