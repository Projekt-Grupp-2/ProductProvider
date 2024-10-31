using ProductProvider.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductProvider.Infrastructure.Models;

public  class ReviewUpdateRequest
{
    public Guid Id { get; set; }

    public Guid? ProductId { get; set; }

    public int? Stars { get; set; }

    public string? Text { get; set; }
}