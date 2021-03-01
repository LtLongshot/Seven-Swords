using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SevenSwords.CharacterCore {
	public class StateInputs
	{
		public enum Inputs { vertical, horizontal, attack, attack2, jump }
		private List<(Inputs input, float value)> inputList = new List<(Inputs, float)>();
		public List<(Inputs input, float value)> _inputList { get => inputList;}

		public void InputUpdate()
		{

		}

		public void clearList()
		{
			inputList.Clear();
		}

		/// <summary>
		/// Horizontal movement controlls
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


}
}

