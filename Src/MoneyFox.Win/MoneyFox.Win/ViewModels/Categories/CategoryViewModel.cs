﻿namespace MoneyFox.Win.ViewModels.Categories;

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Aggregates;
using Core.Aggregates.CategoryAggregate;
using Core.Common.Interfaces.Mapping;

public class CategoryViewModel : ObservableObject, IMapFrom<Category>
{
    private int id;
    private string name = "";
    private string note = "";
    private bool requireNote;
    private DateTime created;
    private DateTime lastModified;

    public int Id
    {
        get => id;

        set
        {
            if (id == value)
            {
                return;
            }

            id = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => name;

        set
        {
            if (name == value)
            {
                return;
            }

            name = value;
            OnPropertyChanged();
        }
    }

    public bool RequireNote
    {
        get => requireNote;

        set
        {
            if (requireNote == value)
            {
                return;
            }

            requireNote = value;
            OnPropertyChanged();
        }
    }

    public DateTime Created
    {
        get => created;

        set
        {
            if (created == value)
            {
                return;
            }

            created = value;
            OnPropertyChanged();
        }
    }

    public DateTime LastModified
    {
        get => lastModified;

        set
        {
            if (lastModified == value)
            {
                return;
            }

            lastModified = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    ///     Additional details about the CategoryViewModel
    /// </summary>
    public string Note
    {
        get => note;

        set
        {
            if (note == value)
            {
                return;
            }

            note = value;
            OnPropertyChanged();
        }
    }
}
