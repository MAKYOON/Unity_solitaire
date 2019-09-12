using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
        protected Sprite _sprite;
        protected string _name;
        protected string _color;
        protected int _value;
        protected bool _faceUp;
        protected bool _canDrag;

        public Card()
        {
            this._name = null;
            this._faceUp = false;
            this._canDrag = false;
            this._sprite = null;
            this._color = null;
            this._value = 0;
        }

        public string GetName()
        {
            return this._name;
        }
        public bool GetFaceUp()
        {
            return this._faceUp;
        }
        public bool GetCanDrag()
        {
            return this._canDrag;
        }
        public string GetColor()
        {
            return this._color;
        }
        public int GetValue()
        {
            return this._value;
        }
        public Sprite GetSprite()
        {
            return this._sprite;
        }
        public void SetName(string name)
        {
            this._name = name;
        }
        public void SetFaceUp(bool faceUp)
        {
            this._faceUp = faceUp;
        }
        public void SetCanDrag(bool canDrag)
        {
            this._canDrag = canDrag;
        }
        public void SetSprite(Sprite sprite)
        {
            this._sprite = sprite;
        }
        public void SetColor(string color)
        {
            this._color = color;
        }
        public void SetValue(int value)
        {
            this._value = value;
        }
}