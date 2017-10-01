﻿using System.Data.Entity.ModelConfiguration;
using Domain.Aggregates.Cinemas;

namespace Infrastructure.Configuration
{
    internal class SeatEntityConfiguration : EntityTypeConfiguration<Seat>
    {
        public SeatEntityConfiguration()
        {
            ToTable("Seats", "cine");

            HasKey(x => new { x.ScreenId, x.Row, x.Number });

            Property(x => x.Row)
                .IsRequired();

            Property(x => x.Number)
                .IsRequired();
        }
    }
}