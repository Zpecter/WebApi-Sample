﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.CollectionFixtures;
using Aplication.Queries.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Api.IntegrationTests.Specs.Administration
{
    [Collection(Collections.Database)]
    public class GetCinemas
    {
        private readonly DatabaseFixture _fixture;

        public GetCinemas(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetCinemas_By_Admin_Should_Return_Values()
        {
            const string endpoint = "api/v1/cinemas";
            var response = await _fixture.Server.CreateRequest(endpoint)
                .WithIdentity(Identities.Administrator)
                .GetAsync();

            await response.IsSuccessStatusCodeOrTrow();

            var values = await response.Content.ReadAsAsync<CinemaViewModel[]>();

            values.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetCinemas_By_User_Should_Return_Values()
        {
            const string endpoint = "api/v1/cinemas";
            var response = await _fixture.Server.CreateRequest(endpoint)
                .WithIdentity(Identities.User)
                .GetAsync();

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
