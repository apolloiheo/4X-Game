using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Technology : MonoBehaviour
{
    // private Civilization _civilization; // Owner
    private TechnologyTree _tree;
    private int _technologyCost;
    private int _technologyProgress;
    private List<Technology> _dependencies; // All technologies that must be researched before this tech can be researched
    private List<Technology> _successors; // All technologies that can be researched after this tech is researched
    private bool _researched = false;
    
    public void AddToProgress(int science)
    {
        _technologyProgress += science;

        if (_technologyProgress >= _technologyCost)
        {
            // Complete research
            Complete();
        }
        
        void Complete()
        {
            // Complete research
            _researched = true;
            _tree.ResearchTechnology(this);
        }
    }
    
    public List<Technology> GetSuccessors()
    {
        return _successors;
    }

    // Return true if this has been researched
    public bool IsResearched()
    {
        return _researched;
    }

    // Return true is this can be researched
    public bool IsResearchable()
    {
        // If any dependencies are not researched, return false
        foreach (Technology tech in _dependencies)
        {
            if (!tech.IsResearched())
            {
                return false;
            }
        }

        return true;
    }

}
