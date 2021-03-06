﻿using System;
using System.Collections.Generic;
using System.IO;
using Business.Abstract;
using Core.Utilities.Results;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarImagesController : ControllerBase
    {
        ICarImageService _carImageService;
        public CarImagesController(ICarImageService carImageService)
        {
            _carImageService = carImageService;
        }

        [HttpPost("add")]
        public IActionResult Add([FromForm(Name = "Image")] IFormFile image, [FromForm(Name = "CarId")] int carId)
        {
            if (image.Length > 0)
            {
                CarImage carImage = new CarImage { CarId = carId };
                AddImage(image, ref carImage);
                var result = _carImageService.Add(carImage);
                if (result.Success)
                {
                    return Ok(result);
                }
                DeleteImage(carImage);
                return BadRequest(result);
            }
            return BadRequest("No image sent.");
        }

        [HttpPost("update")]
        public IActionResult Update([FromForm(Name = "Image")] IFormFile image, [FromForm(Name = "CarImageId")] int carImageId)
        {
            if (image.Length > 0)
            {
                CarImage carImage = new CarImage { CarImageId = carImageId };
                if (!DeleteImage(carImage))
                {
                    return BadRequest("Image couldn't have been deleted.");
                }
                AddImage(image, ref carImage);
                carImage.CarId = getCarImage(carImage).Data.CarId;
                var result = _carImageService.Update(carImage);
                if (result.Success)
                {
                    return Ok(result);
                }
                DeleteImage(carImage);
                return BadRequest(result);
            }
            return BadRequest("No image sent.");
        }

        [HttpPost("delete")]
        public IActionResult Delete(CarImage carImage)
        {
            if (!DeleteImage(carImage))
            {
                return BadRequest("Image couldn't have been deleted.");
            }
            var result = _carImageService.Delete(carImage);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getbyid")]
        public IActionResult GetById([FromForm(Name = "CarImageId")] int carImageId)
        {
            var result = _carImageService.GetById(carImageId);
            if (result.Success)
            {
                if (result.Data != null)
                {
                    return Ok(OpenFile(result.Data.ImagePath));
                }
                return Ok(OpenDefaultImage());
            }
            return BadRequest(result);
        }

        [HttpGet("getallbycarid")]
        public IActionResult GetAllByCarId([FromForm(Name = "carId")] int carId)
        {

            var result = _carImageService.GetAllByCarId(carId);
            if (result.Data.Count > 0)
            {
                return Ok(result);
            }
            else if (result.Data.Count == 0)
            {
                return Ok(OpenDefaultImage());
            }

            return BadRequest(result);
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var result = _carImageService.GetAll();
            if (result.Success)
            {
                return Ok(result);
            }
            else if (result.Data == null)
            {
                return Ok(OpenDefaultImage());
            }
            return BadRequest(result);
        }

        private IDataResult<CarImage> getCarImage(CarImage carImage)
        {
            var result = _carImageService.GetById(carImage.CarImageId);
            return result;
        }

        private bool DeleteImage(CarImage carImage)
        {
            CarImage _carImage = new CarImage();
            var getCarImageResult = getCarImage(carImage);
            if (getCarImageResult.Success)
            {
                _carImage = getCarImageResult.Data;
                System.IO.File.Delete(_carImage.ImagePath);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddImage(IFormFile image, ref CarImage carImage)
        {
            if (image.Length > 0)
            {
                var tempFilePath = Path.GetTempFileName();
                var filePath = Directory.GetParent(Directory.GetCurrentDirectory()) + @"\CarImages\" + Guid.NewGuid() + Path.GetExtension(image.FileName);

                using (FileStream tempFileStream = new FileStream(tempFilePath, FileMode.Create))
                    image.CopyTo(tempFileStream);

                System.IO.File.Move(tempFilePath, filePath);
                carImage.ImagePath = filePath;
                carImage.Date = DateTime.Now;
            }

        }

        private FileStream OpenFile(string filePath)
        {
            return System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private FileStream OpenDefaultImage()
        {
            var defaultFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + @"\CarImages\default.png";
            return OpenFile(defaultFilePath);
        }
    }
}
