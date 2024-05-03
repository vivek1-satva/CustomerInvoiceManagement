﻿using MongoDB.Bson;
        [RegularExpression(@"^[a-zA-Z0-9-]*$", ErrorMessage = "Invoice number must be in correct format")]

        //[Required(ErrorMessage = "At least one line item is required.")]
        //public List<ItemInvoice> LineItems { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total must be a positive number.")]

        [Required(ErrorMessage = "Country is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Invalid country format. Only letters are allowed.")]
        public string Country { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Invalid state  format. Only letters are allowed.")]
        public string State { get; set; }