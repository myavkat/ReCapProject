﻿using System;
using System.Collections.Generic;
using System.Text;
using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class ColorManager : IColorManager
    {
        IColorDal _colorDal;

        public ColorManager(IColorDal colorDal)
        {
            _colorDal = colorDal;
        }

        public void Add(Color entity)
        {
            _colorDal.Add(entity);
        }

        public void Delete(Color entity)
        {
            _colorDal.Delete(entity);
        }

        public List<Color> GetAll()
        {
            return _colorDal.GetAll();
        }

        public Color GetById(int entityId)
        {
            return _colorDal.Get(c => c.ColorId == entityId);
        }

        public void Update(Color entity)
        {
            _colorDal.Update(entity);
        }
    }
}